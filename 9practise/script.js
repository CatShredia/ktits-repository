document.getElementById("myForm").onsubmit = function (event) {
  event.preventDefault();

  let name = document.getElementById("name").value;
  let birthYear = document.getElementById("birthYear").value;
  let currentYear = new Date().getFullYear();
  let age = currentYear - birthYear;

  let isValid = true;

  document.getElementById("nameError").textContent = "";
  document.getElementById("yearError").textContent = "";
  document.getElementById("name").classList.remove("error");
  document.getElementById("birthYear").classList.remove("error");

  if (!name || name.length < 2) {
    document.getElementById("nameError").textContent =
      "Имя не может быть пустым и должно содержать минимум 2 символа.";
    document.getElementById("name").classList.add("error");
    isValid = false;
  }

  if (!birthYear || birthYear.length !== 4 || isNaN(birthYear)) {
    document.getElementById("yearError").textContent =
      "Год рождения должен содержать ровно 4 цифры.";
    document.getElementById("birthYear").classList.add("error");
    isValid = false;
  } else if (age < 18 || age > 120) {
    document.getElementById("yearError").textContent =
      "Вы должны быть старше 18 лет и младше 120.";
    document.getElementById("birthYear").classList.add("error");
    isValid = false;
  }

  if (isValid) {
    alert("Форма успешно отправлена!");
  }
};
