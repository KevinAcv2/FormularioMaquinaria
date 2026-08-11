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

});