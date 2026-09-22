document.addEventListener("DOMContentLoaded", function () {

    // =====================================================
    // MODALES
    // =====================================================

    const elementoModalNueva = document.getElementById("modalNuevaMaquina");
    const elementoModalEditar = document.getElementById("modalEditarMaquina");

    const modalNuevaMaquina = elementoModalNueva
        ? bootstrap.Modal.getOrCreateInstance(elementoModalNueva)
        : null;

    const modalEditarMaquina = elementoModalEditar
        ? bootstrap.Modal.getOrCreateInstance(elementoModalEditar)
        : null;

    // =====================================================
    // MODAL EDITAR VOLQUETA
    // =====================================================

    const elementoModalEditarVolqueta =
        document.getElementById("modalEditarVolqueta");

    const modalEditarVolqueta = elementoModalEditarVolqueta
        ? bootstrap.Modal.getOrCreateInstance(elementoModalEditarVolqueta)
        : null;


    // =====================================================
    // CARGAR DATOS EN MODAL EDITAR VOLQUETA
    // =====================================================

    document.querySelectorAll(".btnEditarVolqueta").forEach(function (boton) {

        boton.addEventListener("click", function () {

            const id = this.dataset.id;
            const placa = this.dataset.placa;
            const propiedad = this.dataset.propiedad;
            const empresa = this.dataset.empresa;
            const estado = this.dataset.estado;
            const observaciones = this.dataset.observaciones;


            document.getElementById("EditarVolquetaId").value = id;

            document.getElementById("EditarPlacaVolqueta").value =
                placa || "";

            document.getElementById("EditarPropiedadVolqueta").value =
                propiedad || "PROPIA";

            document.getElementById("EditarEmpresaPropietariaVolqueta").value =
                empresa || "";

            document.getElementById("EditarEstadoVolqueta").value =
                estado || "1";

            document.getElementById("EditarObservacionesVolqueta").value =
                observaciones || "";


            if (modalEditarVolqueta) {
                modalEditarVolqueta.show();
            }

        });

    });

    // =====================================================
    // GUARDAR EDICIÓN DE VOLQUETA
    // =====================================================

    const formEditarVolqueta =
        document.getElementById("formEditarVolqueta");

    if (formEditarVolqueta) {

        formEditarVolqueta.addEventListener("submit", async function (e) {

            e.preventDefault();


            const datos = {

                Id: parseInt(
                    document.getElementById("EditarVolquetaId").value
                ),

                Placa: document
                    .getElementById("EditarPlacaVolqueta")
                    .value
                    .trim()
                    .toUpperCase(),

                Propiedad: document
                    .getElementById("EditarPropiedadVolqueta")
                    .value,

                EmpresaPropietaria: document
                    .getElementById("EditarEmpresaPropietariaVolqueta")
                    .value
                    .trim(),

                Estado: document
                    .getElementById("EditarEstadoVolqueta")
                    .value,

                Observaciones: document
                    .getElementById("EditarObservacionesVolqueta")
                    .value
                    .trim()

            };


            try {

                const respuesta = await fetch("/Volquetas/Editar", {

                    method: "POST",

                    headers: {
                        "Content-Type": "application/json"
                    },

                    body: JSON.stringify(datos)

                });


                const resultado = await respuesta.json();


                if (!respuesta.ok) {

                    throw new Error(
                        resultado.mensaje ||
                        "No fue posible actualizar la volqueta."
                    );

                }


                if (modalEditarVolqueta) {
                    modalEditarVolqueta.hide();
                }


                await Swal.fire({

                    icon: "success",

                    title: "¡Actualizada!",

                    text: "La volqueta fue actualizada correctamente.",

                    confirmButtonColor: "#198754"

                });


                location.reload();

            }
            catch (error) {

                console.error(
                    "Error al actualizar volqueta:",
                    error
                );


                Swal.fire({

                    icon: "error",

                    title: "Error",

                    text: error.message ||
                        "No fue posible actualizar la volqueta."

                });

            }

        });

    }

    // =====================================================
    // MODAL NUEVA VOLQUETA
    // =====================================================

    const elementoModalNuevaVolqueta =
        document.getElementById("modalNuevaVolqueta");

    const modalNuevaVolqueta = elementoModalNuevaVolqueta
        ? bootstrap.Modal.getOrCreateInstance(elementoModalNuevaVolqueta)
        : null;

    // =====================================================
    // CREAR VOLQUETA
    // =====================================================

    const formNuevaVolqueta =
        document.getElementById("formNuevaVolqueta");

    if (formNuevaVolqueta) {

        formNuevaVolqueta.addEventListener("submit", async function (e) {

            e.preventDefault();

            const datos = {

                Placa: document
                    .getElementById("PlacaVolqueta")
                    .value
                    .trim()
                    .toUpperCase(),

                Propiedad: document
                    .getElementById("PropiedadVolqueta")
                    .value,

                EmpresaPropietaria: document
                    .getElementById("EmpresaPropietariaVolqueta")
                    .value
                    .trim(),

                Estado: "1",

                Observaciones: document
                    .getElementById("ObservacionesVolqueta")
                    .value
                    .trim()
            };


            try {

                const respuesta = await fetch("/Volquetas/Crear", {

                    method: "POST",

                    headers: {
                        "Content-Type": "application/json"
                    },

                    body: JSON.stringify(datos)

                });


                const resultado = await respuesta.json();


                if (!respuesta.ok) {

                    throw new Error(
                        resultado.mensaje ||
                        "No fue posible registrar la volqueta."
                    );

                }


                if (modalNuevaVolqueta) {
                    modalNuevaVolqueta.hide();
                }


                formNuevaVolqueta.reset();


                await Swal.fire({

                    icon: "success",

                    title: "¡Correcto!",

                    text: "La volqueta fue registrada correctamente.",

                    confirmButtonColor: "#198754"

                });


                location.reload();

            }
            catch (error) {

                console.error(
                    "Error al registrar volqueta:",
                    error
                );


                Swal.fire({

                    icon: "error",

                    title: "Error",

                    text: error.message ||
                        "No fue posible registrar la volqueta."

                });

            }

        });

    }


    // =====================================================
    // CREAR MÁQUINA
    // =====================================================

    const formNueva = document.getElementById("formNuevaMaquina");

    if (formNueva) {

        formNueva.addEventListener("submit", async function (e) {

            e.preventDefault();

            const datos = {
                Nombre: document.getElementById("NombreMaquina").value.trim(),
                Estado: document.getElementById("EstadoMaquina").value
            };

            try {

                const respuesta = await fetch("/Maquinas/Crear", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(datos)
                });

                const resultado = await respuesta.json();

                if (!respuesta.ok) {

                    throw new Error(
                        resultado.mensaje ||
                        "No fue posible registrar la máquina."
                    );
                }

                if (modalNuevaMaquina) {
                    modalNuevaMaquina.hide();
                }

                formNueva.reset();

                await Swal.fire({
                    icon: "success",
                    title: "¡Correcto!",
                    text: "La máquina fue registrada correctamente.",
                    confirmButtonColor: "#198754"
                });

                location.reload();

            }
            catch (error) {

                console.error("Error al registrar máquina:", error);

                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: error.message ||
                        "No fue posible registrar la máquina."
                });

            }

        });

    }


    // =====================================================
    // CARGAR DATOS EN MODAL EDITAR
    // =====================================================

    document.querySelectorAll(".btnEditar").forEach(function (boton) {

        boton.addEventListener("click", function () {

            const id = this.dataset.id;
            const nombre = this.dataset.nombre;
            const estado = this.dataset.estado;

            document.getElementById("EditarId").value = id;
            document.getElementById("EditarNombre").value = nombre;
            document.getElementById("EditarEstado").value = estado;

            if (modalEditarMaquina) {
                modalEditarMaquina.show();
            }

        });

    });


    // =====================================================
    // GUARDAR EDICIÓN
    // =====================================================

    const formEditar = document.getElementById("formEditarMaquina");

    if (formEditar) {

        formEditar.addEventListener("submit", async function (e) {

            e.preventDefault();

            const datos = {
                Id: parseInt(
                    document.getElementById("EditarId").value
                ),
                Nombre: document.getElementById("EditarNombre").value.trim(),
                Estado: document.getElementById("EditarEstado").value
            };

            try {

                const respuesta = await fetch("/Maquinas/Editar", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(datos)
                });

                const resultado = await respuesta.json();

                if (!respuesta.ok) {

                    throw new Error(
                        resultado.mensaje ||
                        "No fue posible actualizar la máquina."
                    );
                }

                if (modalEditarMaquina) {
                    modalEditarMaquina.hide();
                }

                await Swal.fire({
                    icon: "success",
                    title: "¡Actualizada!",
                    text: "La máquina fue actualizada correctamente.",
                    confirmButtonColor: "#198754"
                });

                location.reload();

            }
            catch (error) {

                console.error("Error al actualizar máquina:", error);

                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: error.message ||
                        "No fue posible actualizar la máquina."
                });

            }

        });

    }


    // =====================================================
    // ELIMINAR MÁQUINA
    // =====================================================

    document.querySelectorAll(".btnEliminar").forEach(function (boton) {

        boton.addEventListener("click", function (e) {

            e.preventDefault();

            const id = this.dataset.id;

            Swal.fire({
                title: "¿Eliminar máquina?",
                text: "Esta acción no se puede deshacer.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#dc3545",
                cancelButtonColor: "#6c757d",
                confirmButtonText: "Sí, eliminar",
                cancelButtonText: "Cancelar"
            })
                .then(async function (result) {

                    if (!result.isConfirmed) {
                        return;
                    }

                    try {

                        const respuesta = await fetch("/Maquinas/Eliminar", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json"
                            },
                            body: JSON.stringify(parseInt(id))
                        });

                        const resultado = await respuesta.json();

                        if (!respuesta.ok || !resultado.exito) {

                            throw new Error(
                                resultado.mensaje ||
                                "No fue posible eliminar la máquina."
                            );
                        }

                        await Swal.fire({
                            icon: "success",
                            title: "¡Eliminada!",
                            text: "La máquina fue eliminada correctamente.",
                            confirmButtonColor: "#198754"
                        });

                        location.reload();

                    }
                    catch (error) {

                        console.error("Error al eliminar máquina:", error);

                        Swal.fire({
                            icon: "error",
                            title: "Error",
                            text: error.message ||
                                "No fue posible eliminar la máquina."
                        });

                    }

                });

        });

    });

    // =====================================================
    // CAMBIAR ESTADO DE VOLQUETA
    // =====================================================

    document.querySelectorAll(".btnEstadoVolqueta").forEach(function (boton) {

        boton.addEventListener("click", function () {

            const id = this.dataset.id;


            Swal.fire({

                title: "¿Cambiar estado?",

                text: "La volqueta cambiará entre ACTIVA e INACTIVA.",

                icon: "warning",

                showCancelButton: true,

                confirmButtonColor: "#198754",

                cancelButtonColor: "#6c757d",

                confirmButtonText: "Sí, cambiar",

                cancelButtonText: "Cancelar"

            })
                .then(async function (result) {

                    if (!result.isConfirmed) {
                        return;
                    }


                    try {

                        const respuesta = await fetch(
                            "/Volquetas/CambiarEstado",
                            {
                                method: "POST",

                                headers: {
                                    "Content-Type": "application/json"
                                },

                                body: JSON.stringify(parseInt(id))
                            }
                        );


                        const resultado = await respuesta.json();


                        if (!respuesta.ok || !resultado.exito) {

                            throw new Error(
                                resultado.mensaje ||
                                "No fue posible cambiar el estado de la volqueta."
                            );

                        }


                        await Swal.fire({

                            icon: "success",

                            title: "¡Actualizada!",

                            text: resultado.mensaje,

                            confirmButtonColor: "#198754"

                        });


                        location.reload();

                    }
                    catch (error) {

                        console.error(
                            "Error al cambiar estado de volqueta:",
                            error
                        );


                        Swal.fire({

                            icon: "error",

                            title: "Error",

                            text: error.message ||
                                "No fue posible cambiar el estado de la volqueta."

                        });

                    }

                });

        });

    });

    // =====================================================
    // FILTRADO AUTOMÁTICO DE VOLQUETAS
    // =====================================================

    const buscarVolqueta =
        document.getElementById("buscarVolqueta");

    const estadoVolqueta =
        document.getElementById("estadoVolqueta");

    const propiedadVolqueta =
        document.getElementById("propiedadVolqueta");

    const limpiarFiltrosVolqueta =
        document.getElementById("limpiarFiltrosVolqueta");


    function filtrarVolquetas() {

        const texto =
            buscarVolqueta
                ? buscarVolqueta.value.trim().toUpperCase()
                : "";

        const estado =
            estadoVolqueta
                ? estadoVolqueta.value
                : "";

        const propiedad =
            propiedadVolqueta
                ? propiedadVolqueta.value
                : "";


        document.querySelectorAll(".filaVolqueta")
            .forEach(function (fila) {

                const placa =
                    (fila.dataset.placa || "")
                        .toUpperCase();

                const estadoFila =
                    fila.dataset.estado || "";

                const propiedadFila =
                    (fila.dataset.propiedad || "")
                        .toUpperCase();


                const coincidePlaca =
                    placa.includes(texto);

                const coincideEstado =
                    estado === "" ||
                    estadoFila === estado;

                const coincidePropiedad =
                    propiedad === "" ||
                    propiedadFila === propiedad;


                if (
                    coincidePlaca &&
                    coincideEstado &&
                    coincidePropiedad
                ) {

                    fila.style.display = "";

                }
                else {

                    fila.style.display = "none";

                }

            });

    }


    // =====================================================
    // BUSCAR MIENTRAS SE ESCRIBE
    // =====================================================

    if (buscarVolqueta) {

        buscarVolqueta.addEventListener(
            "input",
            filtrarVolquetas
        );

    }


    // =====================================================
    // FILTRAR AL CAMBIAR ESTADO
    // =====================================================

    if (estadoVolqueta) {

        estadoVolqueta.addEventListener(
            "change",
            filtrarVolquetas
        );

    }


    // =====================================================
    // FILTRAR AL CAMBIAR PROPIEDAD
    // =====================================================

    if (propiedadVolqueta) {

        propiedadVolqueta.addEventListener(
            "change",
            filtrarVolquetas
        );

    }


    // =====================================================
    // LIMPIAR FILTROS
    // =====================================================

    if (limpiarFiltrosVolqueta) {

        limpiarFiltrosVolqueta.addEventListener(
            "click",
            function () {

                if (buscarVolqueta) {
                    buscarVolqueta.value = "";
                }

                if (estadoVolqueta) {
                    estadoVolqueta.value = "";
                }

                if (propiedadVolqueta) {
                    propiedadVolqueta.value = "";
                }

                filtrarVolquetas();

            }
        );

    }

});

// =====================================================
// FILTRADO AUTOMÁTICO DE MÁQUINAS
// =====================================================

const buscarMaquina =
    document.getElementById("buscarMaquina");

const estadoMaquinaFiltro =
    document.getElementById("estadoMaquina");

const limpiarFiltrosMaquina =
    document.getElementById("limpiarFiltrosMaquina");


function filtrarMaquinas() {

    const texto =
        buscarMaquina
            ? buscarMaquina.value.trim().toUpperCase()
            : "";

    const estado =
        estadoMaquinaFiltro
            ? estadoMaquinaFiltro.value
            : "";


    document.querySelectorAll(".filaMaquina")
        .forEach(function (fila) {

            const nombre =
                (fila.dataset.nombre || "")
                    .toUpperCase();

            const estadoFila =
                fila.dataset.estado || "";


            const coincideNombre =
                nombre.includes(texto);

            const coincideEstado =
                estado === "" ||
                estadoFila === estado;


            if (coincideNombre && coincideEstado) {
                fila.style.display = "";
            }
            else {
                fila.style.display = "none";
            }

        });

}


if (buscarMaquina) {
    buscarMaquina.addEventListener("input", filtrarMaquinas);
}

if (estadoMaquinaFiltro) {
    estadoMaquinaFiltro.addEventListener("change", filtrarMaquinas);
}

if (limpiarFiltrosMaquina) {

    limpiarFiltrosMaquina.addEventListener(
        "click",
        function () {

            if (buscarMaquina) {
                buscarMaquina.value = "";
            }

            if (estadoMaquinaFiltro) {
                estadoMaquinaFiltro.value = "";
            }

            filtrarMaquinas();

        }
    );

}