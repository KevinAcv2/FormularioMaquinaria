document.addEventListener("DOMContentLoaded", function () {

    const colors = [
        "#0d6efd",
        "#198754",
        "#ffc107",
        "#dc3545",
        "#6f42c1",
        "#20c997"
    ];

    // Datos enviados desde la vista
    const {
        maquinasLabels,
        maquinasData,
        frentesLabels,
        frentesData,
        mesesLabels,
        mesesData
    } = window.estadisticasData;

    // Reportes por Máquina
    const canvasMaquinas = document.getElementById("graficoMaquinas");

    if (canvasMaquinas) {

        new Chart(canvasMaquinas, {
            type: "bar",
            data: {
                labels: maquinasLabels,
                datasets: [{
                    label: "Cantidad de Reportes",
                    data: maquinasData,
                    borderWidth: 1,
                    borderRadius: 10,
                    backgroundColor: colors
                }]
            },
            options: {
                responsive: true,
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

    // Reportes por Frente
    const canvasFrentes = document.getElementById("graficoFrentes");

    if (canvasFrentes) {

        new Chart(canvasFrentes, {
            type: "doughnut",
            data: {
                labels: frentesLabels,
                datasets: [{
                    data: frentesData,
                    backgroundColor: colors
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: "bottom"
                    }
                }
            }
        });

    }

    // Tendencia Semanal
    const canvasSemana = document.getElementById("graficoSemana");

    let graficoSemanal = null;

    if (canvasSemana) {

        graficoSemanal = new Chart(canvasSemana, {
            type: "line",
            data: {
                labels: mesesLabels,
                datasets: [{
                    label: "Reportes",
                    data: mesesData,
                    borderColor: "#0d6efd",
                    backgroundColor: "rgba(13,110,253,.15)",
                    fill: true,
                    tension: 0.4,
                    pointRadius: 5,
                    pointHoverRadius: 7
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
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

    // Calendario
    const calendario = document.getElementById("fechaSemana");

    if (calendario) {

        calendario.valueAsDate = new Date();

        async function cargarSemana(fecha) {

            const respuesta = await fetch(
                `/Estadisticas/ObtenerTendenciaSemanal?fecha=${fecha}`
            );

            const datos = await respuesta.json();

            graficoSemanal.data.labels = datos.labels;
            graficoSemanal.data.datasets[0].data = datos.data;

            graficoSemanal.update();
        }

        // Cargar semana actual
        cargarSemana(calendario.value);

        calendario.addEventListener("change", function () {
            cargarSemana(this.value);
        });

    }

});