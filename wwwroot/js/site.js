// =========================================================
// SITE.JS
// Funcionalidades generales del Layout
// =========================================================

document.addEventListener("DOMContentLoaded", () => {

    // =========================================================
    // SIDEBAR / MENÚ
    // =========================================================

    const btnMenu = document.getElementById("btnMenu");
    const btnCerrarSidebar = document.getElementById("btnCerrarSidebar");

    const sidebar = document.getElementById("sidebar");
    const contenido = document.querySelector(".contenido-layout");


    // =========================================================
    // ABRIR SIDEBAR
    // =========================================================

    if (btnMenu && sidebar) {

        btnMenu.addEventListener("click", () => {

            sidebar.classList.add("sidebar-open");

            if (contenido) {
                contenido.classList.add("sidebar-visible");
            }

        });

    }


    // =========================================================
    // CERRAR SIDEBAR
    // =========================================================

    if (btnCerrarSidebar && sidebar) {

        btnCerrarSidebar.addEventListener("click", () => {

            sidebar.classList.remove("sidebar-open");

            if (contenido) {
                contenido.classList.remove("sidebar-visible");
            }

        });

    }


    // =====================================================
    // NOTIFICACIONES
    // =====================================================

    document.addEventListener("click", async (e) => {

        const boton = e.target.closest(".abrirNotificacion");

        if (!boton)
            return;

        e.preventDefault();

        const id = boton.dataset.id;

        try {

            const respuesta =
                await fetch("/Admin/ObtenerNovedad?id=" + id);

            if (!respuesta.ok)
                return;

            const datos = await respuesta.json();

            console.log(datos);

            const modal =
                document.getElementById("modalNotificacion");

            const header =
                document.querySelector(
                    "#modalNotificacion .modal-header"
                );

            const titulo =
                document.getElementById(
                    "tituloModalNotificacion"
                );

            const contenido =
                document.getElementById(
                    "contenidoNotificacion"
                );


            // =================================================
            // ENCABEZADO DEL MODAL
            // =================================================

            if (datos.estado === "Activa") {

                header.className =
                    "modal-header bg-danger text-white";

                titulo.innerHTML = `
                    <i class="fa-solid fa-triangle-exclamation me-2"></i>
                    Novedad Activa
                `;

            } else {

                header.className =
                    "modal-header bg-success text-white";

                titulo.innerHTML = `
                    <i class="fa-solid fa-circle-check me-2"></i>
                    Novedad Finalizada
                `;

            }


            // =================================================
            // CONTENIDO
            // =================================================

            contenido.innerHTML = `

                <div class="row g-4">

                    <!-- INFORMACIÓN -->
                    <div class="col-lg-6">

                        <h4 class="fw-bold text-primary mb-4">
                            ${datos.maquina}
                        </h4>

                        <table class="table align-middle">

                            <tbody>

                                <tr>
                                    <th style="width:150px;">
                                        Operador
                                    </th>

                                    <td>
                                        ${datos.operador}
                                    </td>
                                </tr>

                                <tr>
                                    <th>
                                        Tipo
                                    </th>

                                    <td>
                                        ${datos.tipo}
                                    </td>
                                </tr>

                                <tr>

                                    <th>
                                        Estado
                                    </th>

                                    <td>

                                        <span class="badge rounded-pill ${datos.estado === "Activa"
                    ? "bg-danger"
                    : "bg-success"
                }">

                                            ${datos.estado}

                                        </span>

                                    </td>

                                </tr>

                            </tbody>

                        </table>


                        <!-- LÍNEA DE TIEMPO -->

                        <div class="border rounded-3 p-3 mb-3">

                            <h6 class="fw-bold mb-3">
                                Línea de tiempo
                            </h6>

                            <div class="d-flex">

                                <div class="me-3 text-center">

                                    <i class="fa-solid fa-circle text-success"></i>

                                    <br>

                                    <i class="fa-solid fa-grip-lines-vertical text-secondary"></i>

                                    <br>

                                    <i class="fa-solid fa-circle ${datos.estado === "Activa"
                    ? "text-danger"
                    : "text-success"
                }"></i>

                                </div>


                                <div>

                                    <div class="mb-4">

                                        <strong>
                                            Inicio
                                        </strong>

                                        <br>

                                        <small class="text-muted">
                                            ${datos.horaInicio}
                                        </small>

                                    </div>


                                    <div>

                                        <strong>
                                            Fin
                                        </strong>

                                        <br>

                                        <small class="text-muted">
                                            ${datos.horaFin ?? "Aún no finaliza"}
                                        </small>

                                    </div>

                                </div>

                            </div>

                        </div>


                        <!-- DURACIÓN -->

                        <div class="border rounded-3 p-3 bg-light mb-3">

                            <div class="d-flex align-items-center">

                                <i class="fa-regular fa-clock text-danger fs-4 me-3"></i>

                                <div>

                                    <small class="text-muted">
                                        Tiempo fuera de servicio
                                    </small>

                                    <h4 class="text-danger fw-bold mb-0">
                                        ${datos.duracion}
                                    </h4>

                                </div>

                            </div>

                        </div>


                        <!-- OBSERVACIÓN -->

                        <div class="border rounded-3 p-3">

                            <h6 class="fw-bold">
                                Observación
                            </h6>

                            <hr>

                            <p class="mb-0">
                                ${datos.observacion}
                            </p>

                        </div>

                    </div>


                    <!-- EVIDENCIA -->
                    <div class="col-lg-6">

                        <div class="card shadow-sm mb-3">

                            <div class="card-header fw-bold">
                                Evidencia inicial
                            </div>

                            <div class="card-body text-center">

                                ${datos.evidenciaInicio

                    ? `
                                        <a href="${datos.evidenciaInicio}"
                                           target="_blank">

                                            <img src="${datos.evidenciaInicio}"
                                                 class="img-fluid rounded"
                                                 style="height:220px;
                                                        width:100%;
                                                        object-fit:cover;
                                                        cursor:pointer;">

                                        </a>
                                      `

                    : "<p class='text-muted'>Sin evidencia.</p>"
                }

                            </div>

                        </div>

                    </div>

                </div>

            `;


            // =================================================
            // MOSTRAR MODAL
            // =================================================

            new bootstrap.Modal(modal).show();

        } catch (error) {

            console.error(
                "Error al cargar la notificación:",
                error
            );

        }

    });

});