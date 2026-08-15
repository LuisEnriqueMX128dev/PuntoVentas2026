(() => {
    const config = window.novaPosConfig;

    if (!config) {
        return;
    }

    const products = new Map(
        (config.productos || []).map(product => [String(product.id).toLowerCase(), product])
    );
    const cart = new Map();

    const scannerInput = document.getElementById('codigoEscaner');
    const scannerButton = document.getElementById('buscarCodigoBtn');
    const manualSearch = document.getElementById('buscarProductoManual');
    const categoryFilter = document.getElementById('filtroCategoria');
    const productCards = [...document.querySelectorAll('[data-product-card]')];
    const emptyState = document.getElementById('productEmptyState');
    const cartList = document.getElementById('cartList');
    const cartSummary = document.getElementById('cartSummary');
    const sidebarBadge = document.getElementById('sidebarCartBadge');
    const subtotalElement = document.getElementById('subtotalValue');
    const taxElement = document.getElementById('taxValue');
    const totalElement = document.getElementById('totalValue');
    const clearButton = document.getElementById('clearCartBtn');
    const cancelButton = document.getElementById('cancelSaleBtn');
    const checkoutButton = document.getElementById('checkoutBtn');
    const confirmPaymentButton = document.getElementById('confirmPaymentBtn');
    const paymentMethod = document.getElementById('paymentMethod');
    const receivedAmount = document.getElementById('receivedAmount');
    const paymentReference = document.getElementById('paymentReference');
    const changeElement = document.getElementById('changeValue');
    const paymentTotalElement = document.getElementById('paymentTotalValue');
    const clientSelect = document.getElementById('clienteVenta');
    const token = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]')?.value;

    const money = value => new Intl.NumberFormat('es-MX', {
        style: 'currency',
        currency: 'MXN'
    }).format(Number(value) || 0);

    const roundMoney = value => Math.round((Number(value) + Number.EPSILON) * 100) / 100;
    const toCents = value => Math.round((Number(value) + Number.EPSILON) * 100);

    const normalizeId = value => String(value || '').toLowerCase();

    const calculateTotals = () => {
        const total = roundMoney([...cart.values()].reduce((sum, item) => sum + roundMoney(Number(item.product.precioVenta) * item.quantity), 0));
        const taxRate = Number(config.porcentajeImpuesto) || 0;
        const tax = roundMoney(taxRate > 0 ? total - (total / (1 + taxRate / 100)) : 0);
        const subtotal = roundMoney(total - tax);

        return { subtotal, tax, total };
    };

    const updateSidebarBadge = quantity => {
        if (sidebarBadge) {
            sidebarBadge.textContent = String(quantity);
        }
    };

    const renderCart = () => {
        const items = [...cart.values()];
        const totalQuantity = items.reduce((sum, item) => sum + item.quantity, 0);
        const totals = calculateTotals();

        if (items.length === 0) {
            cartList.innerHTML = `
                <div class="nova-cart-empty">
                    <span>🛒</span>
                    <strong>La venta está vacía</strong>
                    <p>Escanea un producto o agrégalo desde el catálogo.</p>
                </div>`;
        } else {
            cartList.innerHTML = items.map(item => `
                <div class="nova-cart-item">
                    <div class="min-w-0">
                        <strong title="${escapeHtml(item.product.nombre)}">${escapeHtml(item.product.nombre)}</strong>
                        <small>${escapeHtml(item.product.codigo)} · ${money(item.product.precioVenta)} c/u · ${money(Number(item.product.precioVenta) * item.quantity)}</small>
                    </div>
                    <div class="nova-quantity-control">
                        <button type="button" data-cart-minus="${item.product.id}" aria-label="Restar">−</button>
                        <span>${item.quantity}</span>
                        <button type="button" data-cart-plus="${item.product.id}" aria-label="Agregar">+</button>
                    </div>
                </div>`).join('');
        }

        cartSummary.textContent = `${totalQuantity} ${totalQuantity === 1 ? 'producto agregado' : 'productos agregados'}`;
        subtotalElement.textContent = money(totals.subtotal);
        taxElement.textContent = money(totals.tax);
        totalElement.textContent = money(totals.total);
        checkoutButton.disabled = items.length === 0 || !config.tieneCajaAbierta;
        updateSidebarBadge(totalQuantity);

        document.querySelectorAll('[data-cart-minus]').forEach(button => {
            button.addEventListener('click', () => changeQuantity(button.dataset.cartMinus, -1));
        });

        document.querySelectorAll('[data-cart-plus]').forEach(button => {
            button.addEventListener('click', () => changeQuantity(button.dataset.cartPlus, 1));
        });
    };

    const escapeHtml = value => String(value ?? '')
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#039;');

    const addProduct = product => {
        const id = normalizeId(product.id);
        const stock = Number(product.existencia) || 0;

        if (stock <= 0) {
            window.novaToast?.('El producto está agotado.', 'Sin existencia', true);
            return;
        }

        products.set(id, product);
        const current = cart.get(id);

        if (current) {
            if (current.quantity >= stock) {
                window.novaToast?.(
                    `Solo hay ${stock} unidades disponibles.`,
                    'Stock insuficiente',
                    true
                );
                return;
            }

            current.quantity += 1;
        } else {
            cart.set(id, { product, quantity: 1 });
        }

        renderCart();
        window.novaToast?.(`${product.nombre} se agregó a la venta.`, 'Producto agregado');
    };

    const changeQuantity = (productId, delta) => {
        const id = normalizeId(productId);
        const item = cart.get(id);

        if (!item) {
            return;
        }

        const nextQuantity = item.quantity + delta;
        const stock = Number(item.product.existencia) || 0;

        if (nextQuantity <= 0) {
            cart.delete(id);
        } else if (nextQuantity > stock) {
            window.novaToast?.(
                `Solo hay ${stock} unidades disponibles.`,
                'Stock insuficiente',
                true
            );
            return;
        } else {
            item.quantity = nextQuantity;
        }

        renderCart();
    };

    const clearCart = askConfirmation => {
        if (cart.size > 0 && askConfirmation && !window.confirm('¿Deseas cancelar la venta actual?')) {
            return;
        }

        cart.clear();
        renderCart();
        scannerInput?.focus();
    };

    const filterProducts = () => {
        const text = (manualSearch?.value || '').trim().toLowerCase();
        const category = categoryFilter?.value || '';
        let visible = 0;

        productCards.forEach(card => {
            const matchesText = !text ||
                card.dataset.productName.includes(text) ||
                card.dataset.productCode.includes(text);
            const matchesCategory = !category || card.dataset.productCategory === category;
            const show = matchesText && matchesCategory;

            card.classList.toggle('is-hidden', !show);
            if (show) {
                visible += 1;
            }
        });

        emptyState?.classList.toggle('d-none', visible > 0);
    };

    const findByCode = async () => {
        const code = (scannerInput?.value || '').trim();

        if (!code) {
            window.novaToast?.('Escribe o escanea un código de producto.', 'Código requerido', true);
            scannerInput?.focus();
            return;
        }

        scannerButton.disabled = true;

        try {
            const url = `${config.buscarCodigoUrl}?codigo=${encodeURIComponent(code)}`;
            const response = await fetch(url, {
                headers: { 'Accept': 'application/json' }
            });
            const data = await response.json().catch(() => null);

            if (!response.ok || !data?.exitoso) {
                throw new Error(data?.mensaje || 'No fue posible localizar el producto.');
            }

            addProduct(data.producto);
            scannerInput.value = '';
        } catch (error) {
            window.novaToast?.(
                error.message + ' Usa la búsqueda manual del catálogo.',
                'Producto no encontrado',
                true
            );
            manualSearch?.focus();
            manualSearch?.select();
        } finally {
            scannerButton.disabled = false;
            window.setTimeout(() => scannerInput?.focus(), 80);
        }
    };

    const updateChange = () => {
        const total = calculateTotals().total;
        const received = roundMoney(receivedAmount?.value || 0);
        const methodName = paymentMethod?.selectedOptions[0]?.text?.trim().toLowerCase() || '';
        const isCash = methodName === 'efectivo';
        const change = isCash ? roundMoney(Math.max(received - total, 0)) : 0;

        changeElement.textContent = money(change);
    };

    const openPayment = () => {
        if (!config.tieneCajaAbierta || !config.cajaId) {
            window.novaToast?.('Primero debes abrir la caja.', 'Caja cerrada', true);
            return;
        }

        if (cart.size === 0) {
            window.novaToast?.('Agrega al menos un producto.', 'Venta vacía', true);
            return;
        }

        const total = roundMoney(calculateTotals().total);

        paymentTotalElement.textContent = money(total);
        receivedAmount.value = total.toFixed(2);
        paymentReference.value = '';

        updateChange();

        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModal')).show();
    };

    const confirmPayment = async () => {
        if (confirmPaymentButton.disabled) {
            return;
        }

        const totals = calculateTotals();
        const methodId = paymentMethod?.value;
        const methodName = paymentMethod?.selectedOptions[0]?.text?.trim().toLowerCase() || '';
        const total = roundMoney(totals.total);
        const received = roundMoney(receivedAmount?.value || 0);
        const totalCents = toCents(total);
        const receivedCents = toCents(received);

        if (!methodId) {
            window.novaToast?.('Selecciona un método de pago.', 'Pago incompleto', true);
            return;
        }

        if (methodName === 'efectivo' && receivedCents < totalCents) {
            window.novaToast?.(`El monto recibido (${money(received)}) es menor al total (${money(total)}).`, 'Monto insuficiente', true);
            receivedAmount?.focus();
            return;
        }

        const model = {
            clienteId: clientSelect?.value || null,
            cajaId: config.cajaId,
            descuento: 0,
            observaciones: null,
            detalles: [...cart.values()].map(item => ({
                productoId: item.product.id,
                cantidad: item.quantity,
                descuento: 0
            })),
            pagos: [{
                metodoPagoId: methodId,
                monto: totalCents / 100,
                montoRecibido: methodName === 'efectivo' ? receivedCents / 100 : totalCents / 100,
                referencia: paymentReference?.value?.trim() || null
            }]
        };

        confirmPaymentButton.disabled = true;
        confirmPaymentButton.textContent = 'Procesando...';

        try {
            const response = await fetch(config.registrarVentaUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'RequestVerificationToken': token || ''
                },
                body: JSON.stringify(model)
            });

            const result = await response.json().catch(() => null);

            if (!response.ok || !result?.exitoso) {
                const details = result?.errores?.length ? result.errores.join(' ') : result?.mensaje;
                throw new Error(details || 'No fue posible registrar la venta.');
            }

            bootstrap.Modal.getInstance(document.getElementById('paymentModal'))?.hide();

            clearCart(false);

            window.novaToast?.(`Folio ${result.folio}. Total ${money(result.total)}. Cambio ${money(result.cambio)}.`, 'Venta completada');

            window.setTimeout(() => window.location.reload(), 1500);
        }
        catch (error) {
            window.novaToast?.(error.message, 'Error al cobrar', true);
        }
        finally {
            confirmPaymentButton.disabled = false;
            confirmPaymentButton.textContent = 'Confirmar cobro';
        }
    };

    document.querySelectorAll('[data-add-product]').forEach(button => {
        button.addEventListener('click', () => {
            const product = products.get(normalizeId(button.dataset.addProduct));
            if (product) {
                addProduct(product);
            }
        });
    });

    scannerButton?.addEventListener('click', findByCode);
    scannerInput?.addEventListener('keydown', event => {
        if (event.key === 'Enter') {
            event.preventDefault();
            findByCode();
        }
    });

    document.addEventListener('keydown', event => {
        if (event.key === 'F2') {
            event.preventDefault();
            scannerInput?.focus();
            scannerInput?.select();
        }
    });

    manualSearch?.addEventListener('input', filterProducts);
    categoryFilter?.addEventListener('change', filterProducts);
    clearButton?.addEventListener('click', () => clearCart(true));
    cancelButton?.addEventListener('click', () => clearCart(true));
    checkoutButton?.addEventListener('click', openPayment);
    receivedAmount?.addEventListener('input', updateChange);

    paymentMethod?.addEventListener('change', () => {
        const total = roundMoney(calculateTotals().total);
        receivedAmount.value = total.toFixed(2);
        updateChange();
    });

    confirmPaymentButton?.addEventListener('click', confirmPayment);

    renderCart();
    window.setTimeout(() => scannerInput?.focus(), 120);
})();
