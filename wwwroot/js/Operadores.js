document.addEventListener("DOMContentLoaded", () => {

    // ==========================
    // MODALES
    // ==========================

    const modalEditarOperador =
        new bootstrap.Modal(document.getElementById("modalEditarOperador"));

    const modalNuevoOperador =
        new bootstrap.Modal(document.getElementById("modalNuevoOperador"));

    // ==========================
    // EDITAR OPERADOR
    // ==========================

    document.querySelectorAll(".btnEditar").forEach(btn => {

        btn.addEventListener("click", () => {

            document.getElementById("EditarId").value =
                btn.dataset.id || "";

            document.getElementById("EditarNombre").value =
                btn.dataset.nombre || "";

            document.getElementById("EditarMaquina").value =
                btn.dataset.maquina || "";

            document.getElementById("EditarFrente").value =
                btn.dataset.frenteid || "";

            modalEditarOperador.show();

        });

    });

    // ==========================
    // GUARDAR EDICIÓN
    // ==========================

    document.getElementById("formEditarOperador")
        .addEventListener("submit", async function (e) {

            e.preventDefault();

            const operador = {

                Id: parseInt(document.getElementById("EditarId").value),

                Nombre: document.getElementById("EditarNombre").value,

                MaquinaId:
                    document.getElementById("EditarMaquina").value
                        ? parseInt(document.getElementById("EditarMaquina").value)
                        : null,

                FrenteOperacionalId:
                    document.getElementById("EditarFrente").value
                        ? parseInt(document.getElementById("EditarFrente").value)
                        : null
            };

            const respuesta = await fetch("/Operadores/Editar", {

                method: "POST",

                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },

                body: JSON.stringify(operador)

            });

            if (respuesta.ok) {

                modalEditarOperador.hide();

                location.reload();

            }
            else {

                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "No fue posible actualizar el operador."
                });

            }

        });

    // ==========================
    // NUEVO OPERADOR
    // ==========================

    document.getElementById("formNuevoOperador")
        .addEventListener("submit", async function (e) {

            e.preventDefault();

            const operador = {

                Nombre:
                    document.getElementById("NuevoNombre").value,

                MaquinaId:
                    document.getElementById("NuevaMaquina").value
                        ? parseInt(document.getElementById("NuevaMaquina").value)
                        : null,

                FrenteOperacionalId:
                    document.getElementById("NuevoFrente").value
                        ? parseInt(document.getElementById("NuevoFrente").value)
                        : null
            };

            const respuesta = await fetch("/Operadores/CrearModal", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(operador)
            });

            if (respuesta.ok) {

                modalNuevoOperador.hide();

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

        });

    // ==========================
    // NUEVO FRENTE
    // ==========================

    document.getElementById("btnGuardarFrente")
        .addEventListener("click", async function () {

            const nombre =
                document.getElementById("NombreNuevoFrente")
                    .value
                    .trim();

            if (nombre === "") {

                Swal.fire({
                    icon: "warning",
                    title: "Atención",
                    text: "Ingrese el nombre del frente."
                });

                return;
            }

            const response = await fetch("/FrenteOperacional/Crear", {

                method: "POST",

                headers: {
                    "Content-Type": "application/json"
                },

                body: JSON.stringify({
                    nombre: nombre
                })

            });

            const resultado = await response.json();

            if (resultado.success) {

                const option1 = document.createElement("option");

                option1.value =
                    resultado.id ?? resultado.nombre;

                option1.text =
                    resultado.nombre;

                document
                    .getElementById("NuevoFrente")
                    .appendChild(option1);

                const option2 =
                    option1.cloneNode(true);

                document
                    .getElementById("EditarFrente")
                    .appendChild(option2);

                document.getElementById("NuevoFrente").value =
                    option1.value;

                document.getElementById("NombreNuevoFrente").value =
                    "";

                bootstrap.Modal
                    .getInstance(document.getElementById("modalNuevoFrente"))
                    .hide();

                Swal.fire({
                    icon: "success",
                    title: "Correcto",
                    text: "Frente agregado correctamente."
                });

            }
            else {

                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: resultado.mensaje
                });

            }

        });

    // ==========================
    // IMPRIMIR
    // ==========================

    const btnImprimir =
        document.getElementById("btnImprimir");

    if (btnImprimir) {

        btnImprimir.addEventListener("click", () => {

            window.print();

        });

    }

    // ==========================
    // ELIMINAR OPERADOR
    // ==========================

    document.querySelectorAll(".btnEliminar").forEach(btn => {

        btn.addEventListener("click", function (e) {

            e.preventDefault();

            let id = this.dataset.id;

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

                if (!result.isConfirmed)
                    return;

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

                    });

            });

        });

    });
    // ELIMINAR FRENTE


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

});