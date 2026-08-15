document.addEventListener("DOMContentLoaded", function () {

    const canvas =
        document.getElementById(
            "dashboardSalesChart");

    if (!canvas) {
        return;
    }

    if (!window.novaDashboardData) {
        return;
    }

    const data =
        window.novaDashboardData;

    new Chart(canvas, {
        type: "line",

        data: {
            labels: data.labels,

            datasets: [
                {
                    label: "Ingresos",

                    data: data.values,

                    borderColor: "#168cff",

                    backgroundColor:
                        "rgba(22, 140, 255, .16)",

                    borderWidth: 3,

                    fill: true,

                    tension: 0.35,

                    pointRadius: 4,

                    pointHoverRadius: 7,

                    pointBackgroundColor:
                        "#07111c",

                    pointBorderColor:
                        "#168cff",

                    pointBorderWidth: 2,

                    pointHoverBackgroundColor:
                        "#168cff",

                    pointHoverBorderColor:
                        "#ffffff",

                    pointHoverBorderWidth: 2
                }
            ]
        },

        options: {
            responsive: true,

            maintainAspectRatio: false,

            interaction: {
                mode: "index",
                intersect: false
            },

            plugins: {
                legend: {
                    display: false
                },

                tooltip: {
                    enabled: true,

                    backgroundColor:
                        "#0f1f30",

                    titleColor:
                        "#ffffff",

                    bodyColor:
                        "#c5d3e3",

                    borderColor:
                        "rgba(255,255,255,.12)",

                    borderWidth: 1,

                    padding: 12,

                    displayColors: false,

                    callbacks: {

                        title: function (
                            tooltipItems) {

                            const index =
                                tooltipItems[0]
                                    .dataIndex;

                            return data.dates[index];
                        },

                        label: function (
                            context) {

                            const index =
                                context.dataIndex;

                            const cantidad =
                                data.quantities[index];

                            const total =
                                Number(
                                    context.raw);

                            return [
                                `Ventas: ${cantidad}`,
                                `Ingresos: ${total.toLocaleString(
                                    "es-MX",
                                    {
                                        style:
                                            "currency",
                                        currency:
                                            "MXN"
                                    }
                                )}`
                            ];
                        }
                    }
                }
            },

            scales: {
                x: {
                    grid: {
                        display: false
                    },

                    ticks: {
                        color: "#8fa1b6"
                    },

                    border: {
                        display: false
                    }
                },

                y: {
                    beginAtZero: true,

                    grid: {
                        color:
                            "rgba(255,255,255,.06)"
                    },

                    ticks: {
                        color: "#8fa1b6",

                        callback:
                            function (value) {

                                return value
                                    .toLocaleString(
                                        "es-MX",
                                        {
                                            style:
                                                "currency",

                                            currency:
                                                "MXN",

                                            maximumFractionDigits:
                                                0
                                        }
                                    );
                            }
                    },

                    border: {
                        display: false
                    }
                }
            }
        }
    });
});