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

    if (btnMenu && sidebar) {
        btnMenu.addEventListener("click", () => {
            sidebar.classList.add("sidebar-open");
            if (contenido) {
                contenido.classList.add("sidebar-visible");
            }
        });
    }

    if (btnCerrarSidebar && sidebar) {
        btnCerrarSidebar.addEventListener("click", () => {
            sidebar.classList.remove("sidebar-open");
            if (contenido) {
                contenido.classList.remove("sidebar-visible");
            }
        });
    }

    // =====================================================
    // CARGAR NOTIFICACIONES AL INICIAR LA PÁGINA
    // =====================================================
    cargarNotificacionesPendientes();


    // =====================================================
    // EVENTO MODAL NOTIFICACIÓN
    // =====================================================

    document.addEventListener("click", async (e) => {
        const boton = e.target.closest(".abrirNotificacion");
        if (!boton) return;

        e.preventDefault();
        const id = boton.dataset.id;

        try {
            const respuesta = await fetch("/Admin/ObtenerNovedad?id=" + id);
            if (!respuesta.ok) return;

            const datos = await respuesta.json();
            const modal = document.getElementById("modalNotificacion");
            const header = document.querySelector("#modalNotificacion .modal-header");
            const titulo = document.getElementById("tituloModalNotificacion");
            const contenidoModal = document.getElementById("contenidoNotificacion");

            if (datos.estado === "Activa") {
                header.className = "modal-header bg-danger text-white";
                titulo.innerHTML = `<i class="fa-solid fa-triangle-exclamation me-2"></i> Novedad Activa`;
            } else {
                header.className = "modal-header bg-success text-white";
                titulo.innerHTML = `<i class="fa-solid fa-circle-check me-2"></i> Novedad Finalizada`;
            }

            contenidoModal.innerHTML = `
                <div class="row g-4">
                    <div class="col-lg-6">
                        <h4 class="fw-bold text-primary mb-4">${datos.maquina}</h4>
                        <table class="table align-middle">
                            <tbody>
                                <tr>
                                    <th style="width:150px;">Operador</th>
                                    <td>${datos.operador}</td>
                                </tr>
                                <tr>
                                    <th>Tipo</th>
                                    <td>${datos.tipo}</td>
                                </tr>
                                <tr>
                                    <th>Estado</th>
                                    <td>
                                        <span class="badge rounded-pill ${datos.estado === "Activa" ? "bg-danger" : "bg-success"}">
                                            ${datos.estado}
                                        </span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="border rounded-3 p-3 mb-3">
                            <h6 class="fw-bold mb-3">Línea de tiempo</h6>
                            <div class="d-flex">
                                <div class="me-3 text-center">
                                    <i class="fa-solid fa-circle text-success"></i><br>
                                    <i class="fa-solid fa-grip-lines-vertical text-secondary"></i><br>
                                    <i class="fa-solid fa-circle ${datos.estado === "Activa" ? "text-danger" : "text-success"}"></i>
                                </div>
                                <div>
                                    <div class="mb-4">
                                        <strong>Inicio</strong><br>
                                        <small class="text-muted">${datos.horaInicio}</small>
                                    </div>
                                    <div>
                                        <strong>Fin</strong><br>
                                        <small class="text-muted">${datos.horaFin ?? "Aún no finaliza"}</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="border rounded-3 p-3 bg-light mb-3">
                            <div class="d-flex align-items-center">
                                <i class="fa-regular fa-clock text-danger fs-4 me-3"></i>
                                <div>
                                    <small class="text-muted">Tiempo fuera de servicio</small>
                                    <h4 class="text-danger fw-bold mb-0">${datos.duracion}</h4>
                                </div>
                            </div>
                        </div>
                        <div class="border rounded-3 p-3">
                            <h6 class="fw-bold">Observación</h6>
                            <hr>
                            <p class="mb-0">${datos.observacion}</p>
                        </div>
                    </div>
                    <div class="col-lg-6">
                        <div class="card shadow-sm mb-3">
                            <div class="card-header fw-bold">Evidencia inicial</div>
                            <div class="card-body text-center">
                                ${datos.evidenciaInicio ? `
                                    <a href="${datos.evidenciaInicio}" target="_blank">
                                        <img src="${datos.evidenciaInicio}" class="img-fluid rounded" style="height:220px; width:100%; object-fit:cover; cursor:pointer;">
                                    </a>
                                ` : "<p class='text-muted'>Sin evidencia.</p>"}
                            </div>
                        </div>
                    </div>
                </div>
            `;

            new bootstrap.Modal(modal).show();
        } catch (error) {
            console.error("Error al cargar la notificación:", error);
        }
    });

});


// =========================================================
// FUNCIONES GLOBALES DE NOTIFICACIONES (FUERA DEL DOMContentLoaded)
// =========================================================

// Función global para disparar la notificación con sonido y animación
function recibirNotificacion(titulo, mensaje, idReporte = null) {
    const icono = document.getElementById("iconoCampana");
    const contador = document.getElementById("contadorNotificaciones");
    const badgeTotal = document.getElementById("badgeTotalNotif");
    const audio = document.getElementById("audioNotificacion");
    const lista = document.getElementById("listaNotificaciones");

    if (!icono) return;

    // 1. Reproducir sonido de alerta
    if (audio) {
        audio.currentTime = 0;
        audio.play().catch(e => console.log("Audio bloqueado por políticas del navegador", e));
    }

    // 2. Activar animación de vibración por 3 segundos
    icono.classList.add("animar-campana");
    setTimeout(() => {
        icono.classList.remove("animar-campana");
    }, 3000);

    // 3. Actualizar contadores numéricos
    let actual = parseInt(contador.innerText) || 0;
    let nuevoTotal = actual + 1;

    contador.innerText = nuevoTotal;
    contador.style.display = "inline-block";

    if (badgeTotal) {
        badgeTotal.innerText = `${nuevoTotal} nuevas`;
    }

    // 4. Limpiar el mensaje de "no hay notificaciones" si existe
    if (lista.innerHTML.includes("No hay notificaciones pendientes")) {
        lista.innerHTML = "";
    }

    // 5. Insertar la nueva notificación en la parte superior de la lista
    let enlaceAttr = idReporte ? `href="javascript:void(0);" data-id="${idReporte}" class="dropdown-item py-3 px-3 border-bottom text-wrap abrirNotificacion"` : `href="javascript:void(0);" class="dropdown-item py-3 px-3 border-bottom text-wrap"`;

    const itemHtml = `
        <li>
            <a ${enlaceAttr}>
                <div class="d-flex w-100 justify-content-between">
                    <strong class="mb-1 text-danger" style="font-size: 13px;">
                        <i class="fa-solid fa-triangle-exclamation me-1"></i> ${titulo}
                    </strong>
                    <small class="text-muted" style="font-size: 10px;">Ahora</small>
                </div>
                <p class="mb-0 text-secondary small">${mensaje}</p>
            </a>
        </li>
    `;
    lista.insertAdjacentHTML('afterbegin', itemHtml);
}

// Función para consultar las notificaciones desde el backend al cargar la página
async function cargarNotificacionesPendientes() {
    try {
        const respuesta = await fetch("/Admin/ObtenerNotificacionesPendientes");
        if (!respuesta.ok) return;

        const notificaciones = await respuesta.json();
        if (notificaciones && notificaciones.length > 0) {
            const contador = document.getElementById("contadorNotificaciones");
            const badgeTotal = document.getElementById("badgeTotalNotif");
            const lista = document.getElementById("listaNotificaciones");

            if (lista.innerHTML.includes("No hay notificaciones pendientes")) {
                lista.innerHTML = "";
            }

            contador.innerText = notificaciones.length;
            contador.style.display = "inline-block";
            if (badgeTotal) badgeTotal.innerText = `${notificaciones.length} nuevas`;

            notificaciones.forEach(n => {
                const itemHtml = `
                    <li>
                        <a href="javascript:void(0);" data-id="${n.reporteMaquinariaId}" class="dropdown-item py-3 px-3 border-bottom text-wrap abrirNotificacion">
                            <div class="d-flex w-100 justify-content-between">
                                <strong class="mb-1 text-danger" style="font-size: 13px;">
                                    <i class="fa-solid fa-triangle-exclamation me-1"></i> ${n.titulo}
                                </strong>
                                <small class="text-muted" style="font-size: 10px;">${n.fecha}</small>
                            </div>
                            <p class="mb-0 text-secondary small">${n.mensaje}</p>
                        </a>
                    </li>
                `;
                lista.insertAdjacentHTML('beforeend', itemHtml);
            });
        }
    } catch (error) {
        console.error("Error al cargar notificaciones pendientes:", error);
    }
}

function marcarTodasLeidas() {
    const contador = document.getElementById("contadorNotificaciones");
    const badgeTotal = document.getElementById("badgeTotalNotif");
    const lista = document.getElementById("listaNotificaciones");

    if (contador) contador.style.display = "none";
    if (contador) contador.innerText = "0";
    if (badgeTotal) badgeTotal.innerText = "0 nuevas";

    if (lista) {
        lista.innerHTML = `
            <li class="p-4 text-center text-muted small">
                <i class="fa-solid fa-check-circle fa-2x mb-2 text-success opacity-50"></i>
                <p class="mb-0">No hay notificaciones pendientes</p>
            </li>
        `;
    }

    // Opcional: Llamada AJAX para marcar como leídas en base de datos si lo deseas
    fetch("/Admin/MarcarNotificacionesLeidas", { method: "POST" }).catch(e => console.log(e));
}