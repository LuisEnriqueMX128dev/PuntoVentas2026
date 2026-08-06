(() => {
    const canvas = document.getElementById('dashboardSalesChart');
    const data = window.novaDashboardData;

    if (!canvas || !data) {
        return;
    }

    const draw = () => {
        const context = canvas.getContext('2d');
        const rectangle = canvas.getBoundingClientRect();
        const ratio = window.devicePixelRatio || 1;
        const width = Math.max(rectangle.width, 300);
        const height = Math.max(rectangle.height, 220);
        const padding = 38;
        const values = data.values.map(Number);
        const maximum = Math.max(...values, 1) * 1.18;

        canvas.width = width * ratio;
        canvas.height = height * ratio;
        context.setTransform(ratio, 0, 0, ratio, 0, 0);
        context.clearRect(0, 0, width, height);

        context.lineWidth = 1;
        context.strokeStyle = 'rgba(255,255,255,.07)';

        for (let index = 0; index < 5; index += 1) {
            const y = padding + ((height - padding * 2) * index / 4);
            context.beginPath();
            context.moveTo(padding, y);
            context.lineTo(width - padding, y);
            context.stroke();
        }

        const points = values.map((value, index) => ({
            x: padding + ((width - padding * 2) * index / Math.max(values.length - 1, 1)),
            y: height - padding - ((height - padding * 2) * value / maximum)
        }));

        const fill = context.createLinearGradient(0, padding, 0, height - padding);
        fill.addColorStop(0, 'rgba(22,140,255,.32)');
        fill.addColorStop(1, 'rgba(22,140,255,0)');

        context.beginPath();
        context.moveTo(points[0].x, height - padding);
        points.forEach(point => context.lineTo(point.x, point.y));
        context.lineTo(points[points.length - 1].x, height - padding);
        context.closePath();
        context.fillStyle = fill;
        context.fill();

        context.beginPath();
        points.forEach((point, index) => {
            if (index === 0) {
                context.moveTo(point.x, point.y);
            } else {
                context.lineTo(point.x, point.y);
            }
        });
        context.strokeStyle = '#168cff';
        context.lineWidth = 3;
        context.stroke();

        points.forEach(point => {
            context.beginPath();
            context.arc(point.x, point.y, 4, 0, Math.PI * 2);
            context.fillStyle = '#07111c';
            context.fill();
            context.strokeStyle = '#29a0ff';
            context.lineWidth = 2;
            context.stroke();
        });

        context.fillStyle = '#8093aa';
        context.font = '12px system-ui';
        context.textAlign = 'center';
        data.labels.forEach((label, index) => {
            context.fillText(label, points[index].x, height - 9);
        });
    };

    draw();
    window.addEventListener('resize', () => window.setTimeout(draw, 100));
})();
