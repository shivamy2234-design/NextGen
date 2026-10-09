// NAVBAR
const menuToggle = document.querySelector(".menu-toggle");
const navLinks = document.querySelector(".nav-links");

if (menuToggle && navLinks) {
    menuToggle.addEventListener("click", () => {
        navLinks.classList.toggle("show");
    });
}
const enquiryForm = document.getElementById("enquiryForm");

if (enquiryForm) {
    enquiryForm.addEventListener("submit", function (e) {
        e.preventDefault();

        const name = document.getElementById("enquiryName").value.trim();
        const phone = document.getElementById("enquiryPhone").value.trim();
        const message = document.getElementById("enquiryMessage").value.trim();

        if (name === "" || phone === "" || message === "") {
            alert("Please fill all fields.");
            return;
        }

        alert("Enquiry submitted successfully.");

        enquiryForm.reset();

        const modalElement = document.getElementById("enquiryModal");
        const modalInstance = bootstrap.Modal.getInstance(modalElement);
        if (modalInstance) {
            modalInstance.hide();
        }
    });
}

    // QUOTE
async function loadQuote() {
    const q = document.getElementById("dailyQuote");
    const a = document.getElementById("quoteAuthor");

    try {
        let res = await fetch("https://zenquotes.io/api/random");
        let data = await res.json();

        q.textContent = `"${data[0].q}"`;
        a.textContent = `— ${data[0].a}`;
    } catch {
        q.textContent = "Keep learning & Keep growing";
        a.textContent = "— NextGen";
    }
}
loadQuote();

// FAQ
document.querySelectorAll(".faq-item").forEach(item => {
    item.querySelector(".faq-question").onclick = () => {
        item.classList.toggle("active");
    };
});

// SLIDER
const track = document.getElementById("teacherTrack");
const prevBtn = document.getElementById("prevBtn");
const nextBtn = document.getElementById("nextBtn");

let index = 0;

function getCardsPerView() {
    if (window.innerWidth <= 768) return 1;
    if (window.innerWidth <= 900) return 2;
    return 3;
}

function updateSlider() {
    const cards = document.querySelectorAll(".teacher-card");
    const max = cards.length - getCardsPerView();

    if (index < 0) index = 0;
    if (index > max) index = max;

    const width = cards[0].offsetWidth + 24;
    track.style.transform = `translateX(-${index * width}px)`;

    prevBtn.disabled = index === 0;
    nextBtn.disabled = index === max;
}

nextBtn.onclick = () => { index++; updateSlider(); };
prevBtn.onclick = () => { index--; updateSlider(); };

window.onload = updateSlider;
window.onresize = updateSlider;