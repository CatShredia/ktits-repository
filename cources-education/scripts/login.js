// login menu
$(document).ready(function () {
  const $loginButton = $("#login-button");
  const $loginModal = $("#login-modal");
  const $closeButton = $(".close-button");

  $loginButton.on("click", function () {
    $loginModal.addClass("show");
  });

  $closeButton.on("click", function () {
    $loginModal.removeClass("show");
  });

  $(window).on("click", function (event) {
    if (event.target == $loginModal[0]) {
      $loginModal.removeClass("show");
    }
  });
});
