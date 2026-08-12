// MENSAJE DE ÉXITO
if (window.mensajeExito && window.mensajeExito.trim() !== "") {

    // Limpiar novedades del reporte anterior
    //localStorage.removeItem("novedadesReporte");

    Swal.fire({
        icon: "success",
        title: "¡Reporte enviado!",
        text: window.mensajeExito,
        confirmButtonColor: "#0f2f44",
        confirmButtonText: "Aceptar"
    });

}

// NOVEDADES DEL REPORTE

let novedades = [];

let evidenciaNovedadFile = null;
function actualizarListaNovedades() {

    const lista = document.getElementById("listaNovedades");

    if (!lista)
        return;

    if (novedades.length === 0) {

        lista.innerHTML = `
            <div class="alert alert-light text-muted">
                No hay novedades registradas.
            </div>
        `;

        return;
    }

    lista.innerHTML = "";

    novedades.forEach((novedad, index) => {

        lista.innerHTML += `
        <div class="card border-warning mb-2">

        <div class="card-body">

            <h6 class="fw-bold text-warning mb-2">
                ${novedad.TipoNovedad}
            </h6>

            <p class="mb-2">
                ${novedad.Observacion}
            </p>

            ${novedad.EvidenciaPreview
                ? `
            <img src="${novedad.EvidenciaPreview}"
                 class="rounded mb-3"
                 style="width:150px; height:150px; object-fit:cover; display:block;">
          `
                : ""
            }   

            <button
                type="button"
                class="btn btn-sm btn-danger"
                onclick="eliminarNovedad(${index})">

                Eliminar

            </button>

        </div>

    </div>
    `;

    });

}

function eliminarNovedad(index) {

    Swal.fire({

        title: "¿Eliminar novedad?",

        text: "Esta novedad será eliminada del reporte.",

        icon: "warning",

        showCancelButton: true,

        confirmButtonText: "Sí, eliminar",

        cancelButtonText: "Cancelar",

        confirmButtonColor: "#d33",

        cancelButtonColor: "#6c757d"

    }).then((result) => {

        if (!result.isConfirmed)
            return;

        novedades.splice(index, 1);

        //localStorage.setItem(
        //    "novedadesReporte",
        //    JSON.stringify(novedades)
        //);

        actualizarListaNovedades();

        Swal.fire({

            icon: "success",

            title: "Novedad eliminada",

            timer: 1200,

            showConfirmButton: false

        });

    });

}

document.addEventListener("DOMContentLoaded", function () {

    actualizarListaNovedades();

    async function guardarNovedad() {
        const tipo = document.getElementById("TipoNovedad").value;
        const observacion = document.getElementById("ObservacionNovedad").value.trim();
        const evidencia = document.getElementById("EvidenciaNovedad").files[0];

        if (tipo === "") {
            Swal.fire({ icon: "warning", title: "Seleccione un tipo de novedad" });
            return;
        }

        const nuevaNovedad = {
            TipoNovedad: tipo,
            Observacion: observacion,
            HoraInicio: new Date().toISOString(),
            HoraFin: null,
            Activa: true,
            EvidenciaPreview: evidencia ? URL.createObjectURL(evidencia) : null
        };

        novedades.push(nuevaNovedad);

        if (evidencia) {
            evidenciaNovedadFile = evidencia;
        }

        actualizarListaNovedades();

        Swal.fire({

            icon: "success",

            title: "Novedad registrada",

            timer: 1200,

            showConfirmButton: false

        });

        document.getElementById("TipoNovedad").value = "";
        document.getElementById("ObservacionNovedad").value = "";
        document.getElementById("EvidenciaNovedad").value = "";

        bootstrap.Modal
            .getInstance(document.getElementById("modalNovedad"))
            .hide();

    }

    document
        .getElementById("btnGuardarNovedad")
        .addEventListener("click", guardarNovedad);

    // ESTADO DE LA MÁQUINA
    document.getElementById("estadoMaquina")
        .addEventListener("change", function () {

            const noOperativa = this.value == "0";

            // Campos del formulario
            const horometroInicial = document.getElementById("horometroInicial");
            const horometroFinal = document.getElementById("horometroFinal");
            const fotoInicial = document.getElementById("fotoInicial");
            const fotoFinal = document.getElementById("fotoFinal");
            const observaciones = document.getElementById("observaciones");

            // Habilitar o deshabilitar campos
            horometroInicial.disabled = noOperativa;
            horometroFinal.disabled = noOperativa;
            fotoInicial.disabled = noOperativa;
            fotoFinal.disabled = noOperativa;
            observaciones.disabled = noOperativa;

            // Si la máquina NO está operativa
            if (noOperativa) {

                // Limpiar campos
                horometroInicial.value = "";
                horometroFinal.value = "";
                fotoInicial.value = "";
                fotoFinal.value = "";
                observaciones.value = "";

                // Abrir automáticamente el modal
                const modal = new bootstrap.Modal(
                    document.getElementById("modalNovedad")
                );
                modal.show();
            }

        });

    // FOTO HORÓMETRO INICIAL
    document.getElementById("fotoInicial")
        .addEventListener("change", async function () {

            let archivo = this.files[0];

            if (!archivo)
                return;

            Swal.fire({
                title: "Leyendo horómetro...",
                text: "La IA está analizando la imagen.",
                allowOutsideClick: false,
                didOpen: () => Swal.showLoading()
            });

            let formData = new FormData();

            formData.append("imagen", archivo);

            let respuesta = await fetch("/Reporte/LeerHorometro", {
                method: "POST",
                body: formData
            });

            // Pruebaaaaaaaaaaaaaaaaaaaaaaaaaaaa

            console.log("OCR status:", respuesta.status);
            console.log("OCR content-type:", respuesta.headers.get("content-type"));

            let respuestaTexto = await respuesta.text();

            console.log("OCR respuesta:", respuestaTexto);

            let datos;

            try {
                datos = JSON.parse(respuestaTexto);
            } catch (error) {
                Swal.close();

                Swal.fire({
                    icon: "error",
                    title: "Respuesta inválida del servidor",
                    text: "El servidor no devolvió una respuesta JSON válida."
                });

                console.error("Error JSON:", error);
                return;
            }

            let datos = await respuesta.json();

            Swal.close();

            if (datos.exito) {

                let input = document.getElementById("horometroInicial");

                input.value = datos.valor;

                input.classList.remove("is-warning");
                input.classList.add("is-valid");

                Swal.fire({
                    icon: "success",
                    title: "Horómetro inicial detectado",
                    html: `
                            <h2 style="color:#0f2f44">${datos.valor}</h2>

                            <p>
                                Revise cuidadosamente el valor detectado.
                                Si observa alguna diferencia puede editarlo
                                antes de guardar el reporte.
                            </p>
                        `,
                    confirmButtonText: "Entendido",
                    confirmButtonColor: "#0f2f44"
                });

            } else {

                Swal.fire({
                    icon: "error",
                    title: "No fue posible leer el horómetro",
                    text: datos.mensaje
                });

            }

        });

    // SI EL OPERADOR MODIFICA EL VALOR
    document.getElementById("horometroInicial")
        .addEventListener("input", function () {

            this.classList.remove("is-valid");
            this.classList.add("is-warning");

        });

    // FOTO HORÓMETRO FINAL
    document.getElementById("fotoFinal")
        .addEventListener("change", async function () {

            let archivo = this.files[0];

            if (!archivo)
                return;

            Swal.fire({
                title: "Leyendo horómetro...",
                text: "La IA está analizando la imagen.",
                allowOutsideClick: false,
                didOpen: () => Swal.showLoading()
            });

            let formData = new FormData();

            formData.append("imagen", archivo);

            let respuesta = await fetch("/Reporte/LeerHorometro", {
                method: "POST",
                body: formData
            });

            let datos = await respuesta.json();

            Swal.close();

            if (datos.exito) {

                let input = document.getElementById("horometroFinal");

                input.value = datos.valor;

                input.classList.remove("is-warning");
                input.classList.add("is-valid");

                Swal.fire({
                    icon: "success",
                    title: "Horómetro final detectado",
                    html: `
                            <h2 style="color:#0f2f44">${datos.valor}</h2>

                            <p>
                                Revise cuidadosamente el valor detectado.
                                Si observa alguna diferencia puede editarlo
                                antes de guardar el reporte.
                            </p>
                        `,
                    confirmButtonText: "Entendido",
                    confirmButtonColor: "#0f2f44"
                });

            } else {

                Swal.fire({
                    icon: "error",
                    title: "No fue posible leer el horómetro",
                    text: datos.mensaje
                });

            }

        });

    // SI EL OPERADOR MODIFICA EL VALOR
    document.getElementById("horometroFinal")
        .addEventListener("input", function () {

            this.classList.remove("is-valid");
            this.classList.add("is-warning");

        });

    // OPERADOR -> MÁQUINA (ONLINE / OFFLINE)

    document.getElementById("operador")
        .addEventListener("change", async function () {

            let nombre = this.value;

            if (nombre === "") {

                document.getElementById("nombreMaquina").value = "";
                return;

            }

            // SIN INTERNET

            if (!navigator.onLine) {

                let maquinasGuardadas =
                    JSON.parse(localStorage.getItem("maquinasOperadores")) || {};

                if (maquinasGuardadas[nombre]) {

                    document.getElementById("nombreMaquina").value =
                        maquinasGuardadas[nombre];

                } else {

                    document.getElementById("nombreMaquina").value = "";

                    Swal.fire({
                        icon: "warning",
                        title: "Sin conexión",
                        text: "No existe una máquina almacenada para este operador."
                    });

                }

                return;
            }

            // CON INTERNET

            try {

                let respuesta = await fetch(
                    "/Reporte/ObtenerMaquinaOperador?nombre=" +
                    encodeURIComponent(nombre));

                let datos = await respuesta.json();

                if (datos.exito) {

                    document.getElementById("nombreMaquina").value =
                        datos.maquina;

                    // Guardar asignación para modo offline

                    // Consultar si existe una novedad activa para esta máquina
                    let respuestaNovedad = await fetch(
                        "/Reporte/ExisteNovedadActiva?maquina=" +
                        encodeURIComponent(datos.maquina));

                    let novedad = await respuestaNovedad.json();

                    if (novedad.activa) {

                        Swal.fire({

                            icon: "warning",

                            title: "Máquina fuera de servicio",

                            html: `
                                    Esta máquina tiene una <b>novedad activa</b>.<br><br>
                                    Para iniciar una nueva jornada primero debes finalizar la novedad.
                                `,

                            showCancelButton: true,

                            confirmButtonText: "Finalizar novedad",

                            cancelButtonText: "Cancelar"

                        }).then((result) => {

                            if (!result.isConfirmed)
                                return;

                            const modal = new bootstrap.Modal(
                                document.getElementById("modalFinalizarNovedad")
                            );

                            modal.show();

                        });

                    }
                    let maquinasGuardadas =
                        JSON.parse(localStorage.getItem("maquinasOperadores")) || {};

                    maquinasGuardadas[nombre] = datos.maquina;

                    localStorage.setItem(
                        "maquinasOperadores",
                        JSON.stringify(maquinasGuardadas)
                    );

                } else {

                    document.getElementById("nombreMaquina").value = "";

                    Swal.fire({
                        icon: "warning",
                        title: "Máquina no asignada",
                        text: "Este operador no tiene una máquina asignada.",
                        confirmButtonColor: "#0f2f44"
                    });

                }

            } catch {

                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "No fue posible consultar la máquina."
                });

            }

        });

    document
        .getElementById("btnFinalizarNovedad")
        .addEventListener("click", async function () {

            const maquina =
                document.getElementById("nombreMaquina").value;

            const observacion =
                document.getElementById("ObservacionFin").value;

            const evidencia =
                document.getElementById("EvidenciaFin").files[0];

            if (observacion.trim() === "") {

                Swal.fire({
                    icon: "warning",
                    title: "Debe escribir la observación de cierre."
                });

                return;
            }

            let formData = new FormData();

            formData.append("maquina", maquina);
            formData.append("observacionFin", observacion);

            if (evidencia)
                formData.append("evidenciaFin", evidencia);

            const respuesta = await fetch("/Reporte/FinalizarNovedad", {

                method: "POST",

                body: formData

            });

            const datos = await respuesta.json();

            if (datos.exito) {

                bootstrap.Modal
                    .getInstance(document.getElementById("modalFinalizarNovedad"))
                    .hide();

                Swal.fire({

                    icon: "success",

                    title: "Novedad finalizada correctamente"

                });

            } else {

                Swal.fire({

                    icon: "error",

                    title: datos.mensaje

                });

            }

        });

    // MODO OFFLINE

    const formulario = document.getElementById("formReporte");

    formulario.addEventListener("submit", function (e) {

        // Enviar las novedades al servidor
        document.getElementById("NovedadesJson").value =
            JSON.stringify(novedades);

        if (evidenciaNovedadFile) {
            const dt = new DataTransfer();
            dt.items.add(evidenciaNovedadFile);
            document.getElementById("EvidenciaNovedadForm").files = dt.files;
        }

        // Navega para revisar que haya internet
        if (navigator.onLine)
            return;

        // Si no hay internet
        e.preventDefault();

        const datos = {

            operador: document.getElementById("operador").value,

            frente: document.querySelector("[name='FrenteOperacional']").value,

            maquina: document.getElementById("nombreMaquina").value,

            estado: document.getElementById("estadoMaquina").value,

            horometroInicial: document.getElementById("horometroInicial").value,

            horometroFinal: document.getElementById("horometroFinal").value,

            observaciones: document.getElementById("observaciones").value,

            fecha: new Date().toISOString()
        };

        let pendientes =
            JSON.parse(localStorage.getItem("reportesPendientes")) || [];

        pendientes.push(datos);

        localStorage.setItem(
            "reportesPendientes",
            JSON.stringify(pendientes)
        );

        Swal.fire({

            icon: "warning",

            title: "Sin conexión",

            text: "El reporte quedó guardado en este dispositivo y se enviará cuando vuelva el internet."

        });

        formulario.reset();

    });
});