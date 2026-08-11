document.addEventListener("DOMContentLoaded", function () {

    // =====================================================
    // MODAL EVALUACIÓN
    // =====================================================

    const elementoModal = document.getElementById("modalEvaluacion");

    const modalEvaluacion = elementoModal
        ? bootstrap.Modal.getOrCreateInstance(elementoModal)
        : null;


    // =====================================================
    // ABRIR MODAL DE EVALUACIÓN
    // =====================================================

    document.querySelectorAll(".btnEvaluar").forEach(btn => {

        btn.addEventListener("click", function () {

            const operador = this.dataset.operador;
            const maquina = this.dataset.maquina;
            const id = this.dataset.id;

            document.getElementById("lblOperador").innerText = operador;
            document.getElementById("lblMaquina").innerText = maquina;
            document.getElementById("ReporteId").value = id;

            // Limpiar evaluación anterior
            document.querySelectorAll(".rating").forEach(rating => {
                rating.dataset.valor = 0;

                rating.querySelectorAll("i").forEach(star => {
                    star.classList.remove("fa-solid");
                    star.classList.remove("active");
                    star.classList.add("fa-regular");
                });
            });

            document.getElementById("ObservacionSupervisor").value = "";

            if (modalEvaluacion) {
                modalEvaluacion.show();
            }

        });

    });


    // =====================================================
    // CREAR ESTRELLAS
    // =====================================================

    document.querySelectorAll(".rating").forEach(rating => {

        // Evitar crear estrellas duplicadas
        if (rating.children.length > 0) {
            return;
        }

        for (let i = 1; i <= 5; i++) {

            const star = document.createElement("i");

            star.className = "fa-regular fa-star";
            star.dataset.value = i;

            rating.appendChild(star);

        }

    });


    // =====================================================
    // SELECCIONAR ESTRELLAS
    // =====================================================

    document.querySelectorAll(".rating").forEach(rating => {

        const stars = rating.querySelectorAll("i");

        stars.forEach(star => {

            star.addEventListener("click", function () {

                const valor = parseInt(this.dataset.value);

                // Guardar valoración
                rating.dataset.valor = valor;

                stars.forEach(s => {

                    const valorEstrella =
                        parseInt(s.dataset.value);

                    if (valorEstrella <= valor) {

                        s.classList.remove("fa-regular");
                        s.classList.add("fa-solid");
                        s.classList.add("active");

                    }
                    else {

                        s.classList.remove("fa-solid");
                        s.classList.remove("active");
                        s.classList.add("fa-regular");

                    }

                });

            });

        });

    });


    // =====================================================
    // GUARDAR EVALUACIÓN
    // =====================================================

    const btnGuardar =
        document.getElementById("btnGuardarEvaluacion");

    if (btnGuardar) {

        btnGuardar.addEventListener("click", async function () {

            const reporteId =
                parseInt(
                    document.getElementById("ReporteId").value
                );

            const datos = {

                ReporteMaquinariaId: reporteId,

                Horario:
                    obtenerValor("Horario"),

                ManejoMaquinaria:
                    obtenerValor("ManejoMaquinaria"),

                CuidadoEquipo:
                    obtenerValor("CuidadoEquipo"),

                SeguridadIndustrial:
                    obtenerValor("SeguridadIndustrial"),

                Productividad:
                    obtenerValor("Productividad"),

                ReporteNovedades:
                    obtenerValor("ReporteNovedades"),

                ObservacionSupervisor:
                    document.getElementById(
                        "ObservacionSupervisor"
                    ).value.trim()

            };


            // Validar que se haya seleccionado una evaluación
            if (
                datos.Horario === 0 ||
                datos.ManejoMaquinaria === 0 ||
                datos.CuidadoEquipo === 0 ||
                datos.SeguridadIndustrial === 0 ||
                datos.Productividad === 0 ||
                datos.ReporteNovedades === 0
            ) {

                Swal.fire({
                    icon: "warning",
                    title: "Evaluación incompleta",
                    text: "Debe calificar todos los criterios antes de guardar.",
                    confirmButtonColor: "#0f2f44"
                });

                return;
            }


            try {

                btnGuardar.disabled = true;

                const respuesta =
                    await fetch("/Evaluacion/Guardar", {

                        method: "POST",

                        headers: {
                            "Content-Type": "application/json"
                        },

                        body: JSON.stringify(datos)

                    });


                if (!respuesta.ok) {

                    const error =
                        await respuesta.text();

                    throw new Error(
                        error ||
                        "No fue posible guardar la evaluación."
                    );

                }


                if (modalEvaluacion) {
                    modalEvaluacion.hide();
                }


                await Swal.fire({

                    icon: "success",

                    title: "¡Evaluación guardada!",

                    text:
                        "La evaluación del operador se registró correctamente.",

                    confirmButtonColor: "#0f2f44",

                    confirmButtonText: "Aceptar"

                });


                location.reload();

            }
            catch (error) {

                console.error(
                    "Error al guardar evaluación:",
                    error
                );

                Swal.fire({

                    icon: "error",

                    title: "Error",

                    text:
                        error.message ||
                        "No fue posible guardar la evaluación.",

                    confirmButtonColor: "#dc3545"

                });

            }
            finally {

                btnGuardar.disabled = false;

            }

        });

    }


    // =====================================================
    // OBTENER VALOR DE ESTRELLAS
    // =====================================================

    function obtenerValor(nombre) {

        const rating =
            document.querySelector(
                `.rating[data-name="${nombre}"]`
            );

        if (!rating) {
            return 0;
        }

        return parseInt(
            rating.dataset.valor || 0
        );

    }


    // =====================================================
    // IMPRIMIR HISTORIAL
    // =====================================================

    window.imprimirHistorial = async function () {

        try {

            const respuesta =
                await fetch("/Reporte/ExportarPdfHistorial");

            if (!respuesta.ok) {

                Swal.fire({

                    icon: "error",

                    title: "Error",

                    text:
                        "No fue posible generar el PDF."

                });

                return;
            }


            const blob =
                await respuesta.blob();

            const url =
                URL.createObjectURL(blob);


            const iframe =
                document.createElement("iframe");

            iframe.style.position = "fixed";
            iframe.style.right = "0";
            iframe.style.bottom = "0";
            iframe.style.width = "0";
            iframe.style.height = "0";
            iframe.style.border = "0";

            iframe.src = url;

            document.body.appendChild(iframe);


            iframe.onload = function () {

                setTimeout(() => {

                    try {

                        iframe.contentWindow.focus();

                        iframe.contentWindow.print();

                    }
                    catch (e) {

                        console.error(
                            "Error al imprimir:",
                            e
                        );

                    }

                }, 800);

            };

        }
        catch (e) {

            console.error(
                "Error al imprimir historial:",
                e
            );

            Swal.fire({

                icon: "error",

                title: "Error",

                text:
                    "No fue posible imprimir."

            });

        }

    };


    // =====================================================
    // ELIMINAR REPORTE
    // =====================================================

    document.querySelectorAll(".btnEliminar").forEach(btn => {

        btn.addEventListener("click", function () {

            const id = this.dataset.id;


            Swal.fire({

                title: "¿Eliminar reporte?",

                text:
                    "Esta acción no se puede deshacer.",

                icon: "warning",

                showCancelButton: true,

                confirmButtonColor: "#dc3545",

                cancelButtonColor: "#6c757d",

                confirmButtonText:
                    "Sí, eliminar",

                cancelButtonText:
                    "Cancelar"

            }).then(async result => {

                if (!result.isConfirmed) {
                    return;
                }


                try {

                    const respuesta =
                        await fetch(
                            `/Reporte/Eliminar?id=${encodeURIComponent(id)}`,
                            {
                                method: "POST"
                            }
                        );


                    if (!respuesta.ok) {

                        throw new Error(
                            "No fue posible eliminar el reporte."
                        );

                    }


                    const data =
                        await respuesta.json();


                    if (!data.exito) {

                        throw new Error(
                            data.mensaje ||
                            "No fue posible eliminar el reporte."
                        );

                    }


                    await Swal.fire({

                        icon: "success",

                        title:
                            "Reporte eliminado",

                        text:
                            "El reporte fue eliminado correctamente.",

                        confirmButtonColor:
                            "#0f2f44"

                    });


                    location.reload();

                }
                catch (error) {

                    console.error(
                        "Error al eliminar reporte:",
                        error
                    );

                    Swal.fire({

                        icon: "error",

                        title: "Error",

                        text:
                            error.message ||
                            "No fue posible eliminar el reporte.",

                        confirmButtonColor:
                            "#dc3545"

                    });

                }

            });

        });
        // =====================================================
        // FECHAS
        // =====================================================

        document.addEventListener("DOMContentLoaded", function () {

            document.querySelectorAll(".date-input-wrapper").forEach(wrapper => {

                const input = wrapper.querySelector(".date-control");
                const placeholder = wrapper.querySelector(".date-placeholder");

                function actualizarFecha() {

                    if (input.value) {
                        placeholder.classList.add("oculto");
                    }
                    else {
                        placeholder.classList.remove("oculto");
                    }

                }

                actualizarFecha();

                input.addEventListener("change", actualizarFecha);

                input.addEventListener("input", actualizarFecha);

            });

        });

    });

});