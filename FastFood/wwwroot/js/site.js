document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".togglePasswordBtn").forEach(btn => {
        btn.addEventListener("click", function () {
            const inputId = this.getAttribute("data-target");
            const input = document.getElementById(inputId);

            if (input.type === "password") {
                input.type = "text";
                this.textContent = "🙈 Ẩn";
            } else {
                input.type = "password";
                this.textContent = "👁 Xem";
            }
        });
    });
});
