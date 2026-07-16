window.dashboard = (function () {
    const charts = {};

    function initChart(canvasId, labels, data) {
        const ctx = document.getElementById(canvasId).getContext('2d');
        const cfg = {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Ore',
                    data: data,
                    fill: true,
                    backgroundColor: 'rgba(56,189,248,0.12)',
                    borderColor: 'rgba(56,189,248,0.9)',
                    tension: 0.3,
                    pointRadius: 3
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false }
                },
                scales: {
                    x: { grid: { display: false }, ticks: { color: '#cbd5e1' } },
                    y: { grid: { color: 'rgba(148,163,184,0.06)' }, ticks: { color: '#cbd5e1' } }
                }
            }
        };
        if (!ctx) return;
        charts[canvasId] = new Chart(ctx, cfg);
    }

    function updateChart(canvasId, labels, data) {
        const ch = charts[canvasId];
        if (!ch) return initChart(canvasId, labels, data);
        ch.data.labels = labels;
        ch.data.datasets[0].data = data;
        ch.update();
    }

    return { initChart, updateChart };
})();
