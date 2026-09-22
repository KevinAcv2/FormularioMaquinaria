document.addEventListener("DOMContentLoaded", () => {

    // ==========================
    // MODALES
    // ==========================

    const elementoModalEditar = document.getElementById("modalEditarOperador");
    const elementoModalNuevo = document.getElementById("modalNuevoOperador");

    const modalEditarOperador = elementoModalEditar
        ? new bootstrap.Modal(elementoModalEditar)
        : null;

    const modalNuevoOperador = elementoModalNuevo
        ? new bootstrap.Modal(elementoModalNuevo)
        : null;


    // ==========================
    // EDITAR OPERADOR (ABRIR MODAL)
    // ==========================

    document.querySelectorAll(".btnEditar").forEach(btn => {
        btn.addEventListener("click", () => {
            document.getElementById("EditarId").value = btn.dataset.id || "";
            document.getElementById("EditarNombre").value = btn.dataset.nombre || "";
            document.getElementById("EditarMaquina").value = btn.dataset.maquina || "";
            document.getElementById("EditarFrente").value = btn.dataset.frenteid || "";

            if (modalEditarOperador) {
                modalEditarOperador.show();
            }
        });
    });


    // ==========================
    // GUARDAR EDICIÓN
    // ==========================

    const formEditarOperador = document.getElementById("formEditarOperador");

    if (formEditarOperador) {
        formEditarOperador.addEventListener("submit", async function (e) {
            e.preventDefault();

            const operador = {
                Id: parseInt(document.getElementById("EditarId").value),
                Nombre: document.getElementById("EditarNombre").value,
                MaquinaId: document.getElementById("EditarMaquina").value
                    ? parseInt(document.getElementById("EditarMaquina").value)
                    : null,
                FrenteOperacionalId: document.getElementById("EditarFrente").value
                    ? parseInt(document.getElementById("EditarFrente").value)
                    : null
            };

            try {
                const respuesta = await fetch("/Operadores/Editar", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "X-Requested-With": "XMLHttpRequest"
                    },
                    body: JSON.stringify(operador)
                });

                if (respuesta.ok) {
                    if (modalEditarOperador) {
                        modalEditarOperador.hide();
                    }
                    location.reload();
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: "No fue posible actualizar el operador."
                    });
                }
            } catch (error) {
                console.error("Error al actualizar operador:", error);
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "No fue posible actualizar el operador."
                });
            }
        });
    }


    // ==========================
    // NUEVO OPERADOR
    // ==========================

    const formNuevoOperador = document.getElementById("formNuevoOperador");

    if (formNuevoOperador) {
        formNuevoOperador.addEventListener("submit", async function (e) {
            e.preventDefault();

            const operador = {
                Nombre: document.getElementById("NuevoNombre").value,
                MaquinaId: document.getElementById("NuevaMaquina").value
                    ? parseInt(document.getElementById("NuevaMaquina").value)
                    : null,
                FrenteOperacionalId: document.getElementById("NuevoFrente").value
                    ? parseInt(document.getElementById("NuevoFrente").value)
                    : null
            };

            try {
                const respuesta = await fetch("/Operadores/CrearModal", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(operador)
                });

                if (respuesta.ok) {
                    if (modalNuevoOperador) {
                        modalNuevoOperador.hide();
                    }

                    Swal.fire({
                        icon: "success",
                        title: "¡Operador registrado!",
                        text: "El operador fue agregado correctamente.",
                        confirmButtonColor: "#0f2f44",
                        confirmButtonText: "Aceptar"
                    }).then(() => {
                        location.reload();
                    });
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: "No fue posible guardar el operador.",
                        confirmButtonColor: "#dc3545"
                    });
                }
            } catch (error) {
                console.error("Error al registrar operador:", error);
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "No fue posible guardar el operador.",
                    confirmButtonColor: "#dc3545"
                });
            }
        });
    }


    // ==========================
    // ELIMINAR OPERADOR
    // ==========================

    document.querySelectorAll(".btnEliminar").forEach(btn => {
        btn.addEventListener("click", function (e) {
            e.preventDefault();
            const id = this.dataset.id;

            Swal.fire({
                title: "¿Eliminar operador?",
                text: "Esta acción no se puede deshacer.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#6c757d",
                confirmButtonText: "Sí, eliminar",
                cancelButtonText: "Cancelar"
            }).then((result) => {
                if (!result.isConfirmed) return;

                fetch("/Operadores/Eliminar", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded"
                    },
                    body: "id=" + id
                })
                    .then(r => r.json())
                    .then(data => {
                        if (data.exito) {
                            Swal.fire({
                                icon: "success",
                                title: "Operador eliminado",
                                text: "El operador fue eliminado correctamente.",
                                confirmButtonColor: "#0f2f44"
                            }).then(() => {
                                location.reload();
                            });
                        } else {
                            Swal.fire({
                                icon: "error",
                                title: "Error",
                                text: "No fue posible eliminar el operador."
                            });
                        }
                    })
                    .catch(error => {
                        console.error("Error al eliminar operador:", error);
                        Swal.fire({
                            icon: "error",
                            title: "Error",
                            text: "No fue posible eliminar el operador."
                        });
                    });
            });
        });
    });


    // ==========================
    // ELIMINAR FRENTE
    // ==========================

    document.querySelectorAll(".btnEliminarFrente").forEach(btn => {
        btn.addEventListener("click", function (e) {
            e.preventDefault();
            const url = this.href;
            const nombre = this.dataset.nombre || "este frente";

            Swal.fire({
                title: "¿Eliminar frente?",
                text: `Se eliminará "${nombre}". Esta acción no se puede deshacer.`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#dc3545",
                cancelButtonColor: "#6c757d",
                confirmButtonText: "Sí, eliminar",
                cancelButtonText: "Cancelar"
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = url;
                }
            });
        });
    });


    // =====================================================
    // FILTRADO COMBINADO DE OPERADORES (TEXTO + FRENTE)
    // =====================================================

    const buscarOperador = document.getElementById("buscarOperador");
    const filtroFrente = document.getElementById("filtroFrente");
    const btnResetFiltros = document.getElementById("btnResetFiltros");

    function filtrarOperadores() {
        const texto = buscarOperador ? buscarOperador.value.trim().toUpperCase() : "";
        const idFrenteSeleccionado = filtroFrente ? filtroFrente.value.trim() : "";

        let filasVisibles = 0;

        document.querySelectorAll(".filaOperador").forEach(function (fila) {
            // 1. Obtiene el texto de la fila (Nombre, Máquina, Frente)
            const textoFila = fila.textContent.trim().toUpperCase();

            // 2. Obtiene el ID del Frente guardado en el atributo data-frente de la fila
            const idFrenteFila = (fila.dataset.frente || "").trim();

            // Condición para el buscador por texto
            const coincideTexto = texto === "" || textoFila.includes(texto);

            // Condición para el selector de Frente Operacional (por ID)
            const coincideFrente = idFrenteSeleccionado === "" || idFrenteFila === idFrenteSeleccionado;

            // Muestra u oculta la fila si cumple AMBAS condiciones
            if (coincideTexto && coincideFrente) {
                fila.style.display = "";
                filasVisibles++;
            } else {
                fila.style.display = "none";
            }
        });

        // Muestra u oculta el mensaje de "Sin resultados" si existe la fila trSinResultados
        const trSinResultados = document.getElementById("trSinResultados");
        if (trSinResultados) {
            trSinResultados.style.display = filasVisibles === 0 ? "" : "none";
        }
    }


    // =====================================================
    // EVENTOS DE ESCUCHA PARA FILTROS Y RESET
    // =====================================================

    if (buscarOperador) {
        buscarOperador.addEventListener("input", filtrarOperadores);
    }

    if (filtroFrente) {
        filtroFrente.addEventListener("change", filtrarOperadores);
    }

    if (btnResetFiltros) {
        btnResetFiltros.addEventListener("click", () => {
            if (buscarOperador) buscarOperador.value = "";
            if (filtroFrente) filtroFrente.value = "";
            filtrarOperadores();
        });
    }

});