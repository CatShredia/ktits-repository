// Массив карт в js
const russianWords = [
  "яблоко",
  "банан",
  "апельсин",
  "виноград",
  "арбуз",
  "клубника",
  "киви",
  "манго",
  "персик",
  "груша",
  "кошка",
  "собака",
  "птица",
  "рыба",
  "кролик",
  "лошадь",
  "корова",
  "свинья",
  "овца",
  "курица",
  "дом",
  "машина",
  "дерево",
  "солнце",
  "луна",
  "звезда",
  "вода",
  "огонь",
  "земля",
  "воздух",
];

function getRandomEnglishWord(number) {
  let returned = "";
  let randomIndex;

  for (let i = 0; i < number; i++) {
    // Исправляем начальное значение i и условие цикла
    randomIndex = Math.floor(Math.random() * russianWords.length);
    returned += russianWords[randomIndex] + " "; // Добавляем пробел для разделения слов
  }
  return returned.trim(); // Возвращаем строку, убрав лишние пробелы в начале и конце
}

function getRandomNumber(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

function getRandomCost() {
  return Math.floor(Math.random() * 10000); // Генерирует цену от 0 до 9999
}

let cardInformation = {
  title: null,
  text: null,
  cost: null,
  addInformation: function (title, text, cost) {
    this.title = title;
    this.text = text;
    this.cost = cost;
  },
};

let card = $(".array .card"); // Выбираем ОРИГИНАЛЬНУЮ карту

const cardCount = 6;

for (let index = 0; index < cardCount - 1; index++) {
  // Ключевое слово 'new' здесь не нужно. Просто используем тот же объект
  cardInformation.addInformation(
    getRandomEnglishWord(1),
    getRandomEnglishWord(10),
    getRandomCost()
  );

  let newCard = card.clone(true); // Клонируем карту, включая события
  newCard.find(".card-title").text(cardInformation.title); // изменил textContent на .text (jQuery)
  newCard.find(".card-text").text(cardInformation.text); // изменил textContent на .text (jQuery)
  newCard.find(".card-cost").text(cardInformation.cost); // изменил textContent на .text (jQuery)

  card.parent().append(newCard); // Добавляем клонированную карту к родителю
}

console.log(card);
