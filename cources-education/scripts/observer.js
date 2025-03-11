// smooth listing
const containers = document.querySelectorAll(".mycontainer");

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
    rootMargin: "0px",
    threshold: 0.3,
  }
);

containers.forEach((container) => {
  observer.observe(container);
});
