document.addEventListener("DOMContentLoaded", function () {
    initNavbar();
    initEnquiryForm();
    loadQuote();
    initFAQ();
    initTeacherSlider();

    setInterval(loadQuote, 60000);
});

/* NAVBAR */
function initNavbar() {
    const menuToggle = document.querySelector(".menu-toggle");
    const navLinks = document.querySelector(".nav-links");
    const auth = document.querySelector(".auth");
    const dropdowns = document.querySelectorAll(".custom-dropdown");

    if (menuToggle) {
        menuToggle.addEventListener("click", function () {
            navLinks?.classList.toggle("show");
            auth?.classList.toggle("show");
        });
    }

    dropdowns.forEach(function (dropdown) {
        const btn = dropdown.querySelector(".dropdown-btn");

        if (btn) {
            btn.addEventListener("click", function (e) {
                e.stopPropagation();

                dropdowns.forEach(function (item) {
                    if (item !== dropdown) {
                        item.classList.remove("active");
                    }
                });

                dropdown.classList.toggle("active");
            });
        }
    });

    document.addEventListener("click", function () {
        dropdowns.forEach(function (dropdown) {
            dropdown.classList.remove("active");
        });
    });
    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape") {
            dropdowns.forEach(function (dropdown) {
                dropdown.classList.remove("active");
            });
        }
    });
    const currentPath = window.location.pathname;

    document.querySelectorAll(".nav-links a").forEach(link => {

        if (link.getAttribute("href").includes(currentPath)) {
            link.classList.add("active");
        }

    });
}


/* ENQUIRY FORM */
function initEnquiryForm() {
    const enquiryForm = document.getElementById("enquiryForm");

    if (!enquiryForm) return;

    enquiryForm.addEventListener("submit", function (e) {
        e.preventDefault();

        const name = document.getElementById("enquiryName")?.value.trim();
        const phone = document.getElementById("enquiryPhone")?.value.trim();
        const message = document.getElementById("enquiryMessage")?.value.trim();

        if (!name || !phone || !message) {
            alert("Please fill all fields.");
            return;
        }

        alert("Enquiry submitted successfully.");
        enquiryForm.reset();

        const modalElement = document.getElementById("enquiryModal");

        if (modalElement && typeof bootstrap !== "undefined") {
            const modalInstance =
                bootstrap.Modal.getInstance(modalElement) ||
                new bootstrap.Modal(modalElement);

            modalInstance.hide();
        }
    });
}


/* QUOTE API */
async function loadQuote() {
    const q = document.getElementById("dailyQuote");
    const a = document.getElementById("quoteAuthor");

    if (!q || !a) return;

    const fallbackQuotes = [
        { quote: "Keep learning and keep growing.", author: "NextGen" },
        { quote: "Small steps every day lead to big results.", author: "NextGen" },
        { quote: "Success comes from consistent practice, not shortcuts.", author: "NextGen" },
        { quote: "The expert in anything was once a beginner.", author: "Helen Hayes" },
        { quote: "Learn today, lead tomorrow.", author: "NextGen Learning Hub" }
    ];

    try {
        q.textContent = "“Loading quote...”";
        a.textContent = "— Please wait";

        const res = await fetch("https://api.quotable.io/random", {
            method: "GET",
            cache: "no-store"
        });

        if (!res.ok) {
            throw new Error("Quote API failed");
        }

        const data = await res.json();

        q.textContent = `“${data.content}”`;
        a.textContent = `— ${data.author}`;
    } catch (error) {
        const randomQuote =
            fallbackQuotes[Math.floor(Math.random() * fallbackQuotes.length)];

        q.textContent = `“${randomQuote.quote}”`;
        a.textContent = `— ${randomQuote.author}`;

        console.log("Quote API error:", error);
    }
}

/* FAQ */
function initFAQ() {
    const faqItems = document.querySelectorAll(".faq-item");

    faqItems.forEach(function (item) {
        const question = item.querySelector(".faq-question");

        if (question) {
            question.addEventListener("click", function () {
                item.classList.toggle("active");
            });
        }
    });
}

/* TEACHER SLIDER */
function initTeacherSlider() {
    const track = document.getElementById("teacherTrack");
    const prevBtn = document.getElementById("prevBtn");
    const nextBtn = document.getElementById("nextBtn");

    if (!track || !prevBtn || !nextBtn) return;

    let index = 0;

    function getCardsPerView() {
        if (window.innerWidth <= 768) return 1;
        if (window.innerWidth <= 900) return 2;
        return 3;
    }

    function updateSlider() {
        const cards = track.querySelectorAll(".teacher-card");

        if (cards.length === 0) return;

        let max = cards.length - getCardsPerView();

        if (max < 0) max = 0;

        if (index < 0) index = 0;
        if (index > max) index = max;

        const gap = 24;
        const cardWidth = cards[0].offsetWidth + gap;

        track.style.transform = `translateX(-${index * cardWidth}px)`;

        prevBtn.disabled = index === 0;
        nextBtn.disabled = index === max;
    }

    nextBtn.addEventListener("click", function () {
        index++;
        updateSlider();
    });

    prevBtn.addEventListener("click", function () {
        index--;
        updateSlider();
    });

    window.addEventListener("resize", updateSlider);

    updateSlider();
}