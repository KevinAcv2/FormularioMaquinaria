        const modalEvaluacion =
        new bootstrap.Modal(
        document.getElementById("modalEvaluacion"));

        document
        .querySelectorAll(".btnEvaluar")
        .forEach(btn=>{

            btn.addEventListener("click",()=>{

                document
                .getElementById("lblOperador")
                .innerText=btn.dataset.operador;

                document
                .getElementById("lblMaquina")
                .innerText=btn.dataset.maquina;

                document
                .getElementById("ReporteId")
                .value= btn.dataset.id;

                modalEvaluacion.show();

            });

        });

        // Creación de estrellas automáticamente
        document.querySelectorAll(".rating").forEach(rating => {

            for (let i = 1; i <= 5; i++) {

                const star = document.createElement("i");

                star.className = "fa-regular fa-star";

                star.dataset.value = i;

                rating.appendChild(star);

            }

        });

        // ACTIVAR ESTRELLAS

        document.querySelectorAll(".rating").forEach(rating => {

            const stars = rating.querySelectorAll("i");

            stars.forEach(star => {

                star.addEventListener("click", () => {

                    const valor = parseInt(star.dataset.value);

                    // Guardar la calificación
                    rating.dataset.valor = valor;

                    stars.forEach(s => {

                        if (parseInt(s.dataset.value) <= valor) {

                            s.classList.remove("fa-regular");
                            s.classList.add("fa-solid");
                            s.classList.add("active");

                        } else {

                            s.classList.remove("fa-solid");
                            s.classList.remove("active");
                            s.classList.add("fa-regular");

                        }

                    });

                });

            });

        });

        // Selección de estrellas
                const btnGuardar =
        document.getElementById("btnGuardarEvaluacion");

        btnGuardar.addEventListener("click", async () => {

            const datos = {

                ReporteMaquinariaId:
                parseInt(document.getElementById("ReporteId").value),

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
                document.getElementById("ObservacionSupervisor").value

            };

            const respuesta = await fetch("/Evaluacion/Guardar",{

                method:"POST",

                headers:{
                    "Content-Type":"application/json"
                },

                body:JSON.stringify(datos)

            });

            if (respuesta.ok) {

                modalEvaluacion.hide();

                Swal.fire({
                    icon: 'success',
                    title: '¡Evaluación guardada!',
                    text: 'La evaluación del operador se registró correctamente.',
                    confirmButtonColor: '#0f2f44',
                    confirmButtonText: 'Aceptar'
                }).then(() => {

                    location.reload();

            });

        }
        else {

            const error = await respuesta.text();

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: error,
                confirmButtonColor: '#dc3545'
            });

        }

        });
                function obtenerValor(nombre){

            const rating =
            document.querySelector(
            `.rating[data-name="${nombre}"]`);

            return parseInt(
            rating.dataset.valor || 0);

        }

        // Boton Imprimir

async function imprimirHistorial() {

    try {

        const respuesta = await fetch("/Reporte/ExportarPdfHistorial");

        if (!respuesta.ok) {
            Swal.fire({
                icon: "error",
                title: "Error",
                text: "No fue posible generar el PDF."
            });
            return;
        }

        const blob = await respuesta.blob();

        const url = URL.createObjectURL(blob);

        const iframe = document.createElement("iframe");

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

                } catch (e) {

                    console.error(e);

                }

            }, 800);

        };

    }
    catch (e) {

        console.error(e);

        Swal.fire({
            icon: "error",
            title: "Error",
            text: "No fue posible imprimir."
        });

    }

}