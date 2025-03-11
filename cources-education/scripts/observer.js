// smooth listing
const mycontainers = document.querySelectorAll(".mycontainer");

const observer = new IntersectionObserver(
  (entries) => {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        entry.target.classList.add("visible");
      } else {
        entry.target.classList.remove("visible");
      }
    });
  },
  {
    root: null,
    rootMargin: "10px",
    threshold: 0.3,
  }
);

mycontainers.forEach((container) => {
  observer.observe(container);
});
