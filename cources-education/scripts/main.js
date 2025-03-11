// menu adding
$(document).ready(function () {
  let $menu = $(".navmenu");
  function menuOpen() {
    $menu.addClass("show");
  }
  function menuClose() {
    $menu.removeClass("show");
  }
  $("#menuButton").on("click", function () {
    menuOpen();
  });
  $(".navmenu__close").on("click", function () {
    menuClose();
  });
});
$(document).ready(function () {
  let $menu = $(".navmenu");
  function menuOpen() {
    $menu.addClass("show");
  }
  function menuClose() {
    $menu.removeClass("show");
  }
  $("#menuButton").on("click", function () {
    menuOpen();
  });
  $(".navmenu__close").on("click", function () {
    menuClose();
  });
});

// Обработчик для кнопки отзываП
$(".feedback").on("click", function () {
  $(this).closest(".feedback").find(".feedback-content").slideToggle(300); // Плавное открытие/закрытие контента отзыва
});

$(document).on("keyup", function (event) {
  if (event.key === "Escape" || event.key === "Esc") {
    $(".error-message").remove();
    $(".success-message").remove();
    $(".navmenu").removeClass("show");
  }
});
