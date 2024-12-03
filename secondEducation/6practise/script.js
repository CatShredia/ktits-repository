// TODO: 1 task
console.log("----------------");
console.log("Hello World!");

const div = document.querySelectorAll("#div");
console.log(div);

const ul = document.querySelectorAll("#ul");
console.log(ul);

const pit = document.querySelectorAll("#pit");
console.log(pit);

// TODO: 2 task
console.log("----------------");
let liArray = document.querySelectorAll("li");
console.log(liArray);

for (let i = 0; i < liArray.length; i++) {
  liArray[i].className = "list-item";
}

// TODO: 3 task
console.log("----------------");
// Объект для хранения количества тегов
const tagCount = {};

// Получаем все элементы на странице
const allTags = document.querySelectorAll("*");

// Подсчитываем количество каждого тега
allTags.forEach((tag) => {
  const tagName = tag.tagName.toLowerCase(); // Приводим к нижнему регистру для единообразия
  if (tagCount[tagName]) {
    tagCount[tagName] += 1; // Увеличиваем счетчик существующего тега
  } else {
    tagCount[tagName] = 1; // Инициализируем счетчик для нового тега
  }
});

// let number = parseInt(prompt("Введите число"));
let number = 2;

// Находим и выводим теги с количеством повторений равным 4
for (const [tagName, count] of Object.entries(tagCount)) {
  if (count === number) {
    console.log(`Тег <${tagName}> встречается ${count} раз(а).`);
  }
}
// TODO: 4 task
console.log("----------------");

console.log("Запуск");
let elementDiv = document.createElement("div");
console.log(elementDiv);

elementDiv.className = "div-class";

document.body.appendChild(elementDiv);

elementDiv = document.querySelector(".div-class");
console.log(elementDiv);

createNewInput(elementDiv, "input", "text", "Имя");
createNewInput(elementDiv, "input", "email", "Email");
createNewInput(elementDiv, "button", "button", "Отправить Email");

function createNewInput(elementDiv, tagName, attribute0, attribute1) {
  let elementInput = document.createElement("input");

  if (tagName === "input") {
    elementInput.className = "div-input";
    elementInput.setAttribute("type", attribute0);
    elementInput.setAttribute("placeholder", attribute1);
  } else if (tagName === "button") {
    elementInput.className = "div-button";
    elementInput.setAttribute("type", attribute0);
    elementInput.setAttribute("placeholder", attribute1);
  }

  elementDiv.appendChild(elementInput);
}
