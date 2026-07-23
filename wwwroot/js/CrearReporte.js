// MENSAJE DE ÉXITO
if (window.mensajeExito && window.mensajeExito.trim() !== "") {

    Swal.fire({
        icon: "success",
        title: "¡Reporte enviado!",
        text: window.mensajeExito,
        confirmButtonColor: "#0f2f44",
        confirmButtonText: "Aceptar"
    });

}

document.addEventListener("DOMContentLoaded", function () {

    // ESTADO DE LA MÁQUINA
    document.getElementById("estadoMaquina")
    .addEventListener("change", function () {

        let divMotivo = document.getElementById("divMotivo");

        if (this.value == "0") {
            divMotivo.style.display = "block";
        } else {
            divMotivo.style.display = "none";
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

    // =======================================
    // OPERADOR -> MÁQUINA (ONLINE / OFFLINE)
    // =======================================

    document.getElementById("operador")
        .addEventListener("change", async function () {

            let nombre = this.value;

            if (nombre === "") {

                document.getElementById("nombreMaquina").value = "";
                return;

            }

            // ===========================
            // SIN INTERNET
            // ===========================

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

            // ===========================
            // CON INTERNET
            // ===========================

            try {

                let respuesta = await fetch(
                    "/Reporte/ObtenerMaquinaOperador?nombre=" +
                    encodeURIComponent(nombre));

                let datos = await respuesta.json();

                if (datos.exito) {

                    document.getElementById("nombreMaquina").value =
                        datos.maquina;

                    // Guardar asignación para modo offline
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

    // =======================================
    // MODO OFFLINE
    // =======================================

    const formulario = document.getElementById("formReporte");

    formulario.addEventListener("submit", function (e) {

        // Si hay internet, deja que el formulario funcione normalmente
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

            observaciones: document.querySelector("[name='Observaciones']").value,

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