document.addEventListener("DOMContentLoaded", function () {
  const loginButton = document.getElementById("login-button");
  const loginModal = document.getElementById("login-modal");
  const closeButton = document.querySelector(".close-button");

  loginButton.addEventListener("click", function () {
    loginModal.classList.add("show");
  });

  closeButton.addEventListener("click", function () {
    loginModal.classList.remove("show");
  });

  window.addEventListener("click", function (event) {
    if (event.target == loginModal) {
      loginModal.classList.remove("show");
    }
  });
});
