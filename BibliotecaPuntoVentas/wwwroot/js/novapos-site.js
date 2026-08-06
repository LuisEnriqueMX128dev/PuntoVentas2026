(() => {
    const sidebar = document.getElementById('novaSidebar');
    const toggle = document.getElementById('sidebarToggle');
    const backdrop = document.getElementById('sidebarBackdrop');

    const closeSidebar = () => {
        sidebar?.classList.remove('open');
        backdrop?.classList.remove('show');
        document.body.classList.remove('overflow-hidden');
    };

    toggle?.addEventListener('click', () => {
        const willOpen = !sidebar?.classList.contains('open');
        sidebar?.classList.toggle('open', willOpen);
        backdrop?.classList.toggle('show', willOpen);
        document.body.classList.toggle('overflow-hidden', willOpen);
    });

    backdrop?.addEventListener('click', closeSidebar);

    window.addEventListener('resize', () => {
        if (window.innerWidth >= 768) {
            closeSidebar();
        }
    });

    let toastTimer;

    window.novaToast = (message, title = 'Operación completada', isError = false) => {
        const toast = document.getElementById('novaToast');
        const toastTitle = document.getElementById('novaToastTitle');
        const toastText = document.getElementById('novaToastText');
        const toastIcon = document.getElementById('novaToastIcon');

        if (!toast || !toastTitle || !toastText || !toastIcon) {
            return;
        }

        clearTimeout(toastTimer);
        toastTitle.textContent = title;
        toastText.textContent = message;
        toastIcon.textContent = isError ? '!' : '✓';
        toast.classList.toggle('error', isError);
        toast.classList.add('show');

        toastTimer = window.setTimeout(() => {
            toast.classList.remove('show');
        }, 3200);
    };
})();
