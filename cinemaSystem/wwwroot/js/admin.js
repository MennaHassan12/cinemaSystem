document.addEventListener("DOMContentLoaded", function () {

    console.log("Admin Dashboard Loaded ");

    let links = document.querySelectorAll(".nav-link");

    links.forEach(link => {
        link.addEventListener("click", function () {
            links.forEach(l => l.classList.remove("active"));
            this.classList.add("active");
        });
    });

});

    setTimeout(() => {
        document.querySelectorAll('.notification').forEach(n => {
            n.style.transition = "all 0.2s ease";
            n.style.opacity = "0";
            setTimeout(() => n.remove(), 300);
        });
    }, 2000);
