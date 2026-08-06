document.addEventListener("DOMContentLoaded", function () {
    const datos = window.dashboardData || {};
    const maquinas = datos.maquinas || [];
    const horas = datos.horas || [];
    const operadores = datos.operadores || [];
    const reportes = datos.reportes || [];

    const horasChartEl = document.getElementById("horasChart");
    if (horasChartEl) {
        new Chart(horasChartEl, {
            type: "bar",
            data: {
                labels: maquinas,
                datasets: [{
                    label: "Horas Trabajadas",
                    data: horas,
                    backgroundColor: "#0f2f44",
                    borderRadius: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    const operadoresChartEl = document.getElementById("operadoresChart");
    if (operadoresChartEl) {
        new Chart(operadoresChartEl, {
            type: "bar",
            data: {
                labels: operadores,
                datasets: [{
                    label: "Reportes",
                    data: reportes,
                    backgroundColor: "#f0ad00",
                    borderRadius: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            precision: 0
                        }
                    }
                }
            }
        });
    }
});
