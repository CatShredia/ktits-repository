// login menu open
$(document).ready(function () {
  const $loginButton = $("#login-button");
  const $loginModal = $("#login-modal");
  const $closeButton = $(".close-button");

  $loginButton.on("click", function () {
    $loginModal.addClass("show");
  });

  $closeButton.on("click", function () {
    $(".error-message").remove();
    $(".success-message").remove();
    $loginModal.removeClass("show");
  });

  $(window).on("click", function (event) {
    if (event.target == $loginModal[0]) {
      $loginModal.removeClass("show");
    }
  });
});

// form validation
let form = $("form");

form.on("submit", function (event) {
  event.preventDefault();

  // чистим все предыдущие сообщения
  $(".form-group").removeClass("error");
  $(".error-message").remove();
  $(".success-message").remove();

  // получаем значения из формы
  let username = $("#username").val();
  let password = $("#password").val();
  let email = $("#email").val();

  let isValid = true;

  // Валидируем name
  if (username.length < 8 || username.length > 20) {
    showError(
      $("#username").closest(".form-group"),
      "Имя пользователя должно быть от 8 до 20 символов."
    );
    isValid = false;
  }

  // Валидируем пароль
  const passwordRegex =
    /^(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]).{8,20}$/;
  if (!passwordRegex.test(password)) {
    showError(
      $("#password").closest(".form-group"),
      "Пароль должен быть от 8 до 20 символов и содержать цифру и спец. символ."
    );
    isValid = false;
  }

  // Валидируем почту
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(email)) {
    showError(
      $("#email").closest(".form-group"),
      "Пожалуйста, введите корректный email."
    );
    isValid = false;
  }

  if (isValid) {
    showSuccess();
  } else {
    console.log("Form is invalid. Please correct the errors.");
  }
});

function showError(element, message) {
  element.addClass("error");
  const errorMessageElement = $(
    "<div class='error-message'>" + message + "</div>"
  );
  element.append(errorMessageElement);
}

function showSuccess() {
  const successMessageElement = $(
    "<div class='success-message'>Форма успешно отправлена!</div>"
  );
  $(".form__button").after(successMessageElement);
}
