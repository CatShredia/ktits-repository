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

// получение слова рандомно из russianWords
// в зависимости от number изменяется число слов
function getRandomEnglishWord(number) {
  let returned = "";
  let randomIndex;

  for (let i = 0; i < number; i++) {
    randomIndex = Math.floor(Math.random() * russianWords.length);
    returned += russianWords[randomIndex] + " ";
  }

  //   делаем первую букву - заглавной
  returned = returned.charAt(0).toUpperCase() + returned.slice(1);
  return returned.trim();
}

// получаем рандомную стоимость
function getRandomCost() {
  return Math.floor(Math.random() * 10000);
}

// объект с свойствами и функцией заполнения
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

// получаем jquery объект всех нужных карт
let card = $(".array .card");

// количество карт
const cardCount = 6;

// перебираем карты
for (let index = 0; index < cardCount - 1; index++) {
  // заполняем инфу
  cardInformation.addInformation(
    getRandomEnglishWord(1),
    getRandomEnglishWord(10),
    getRandomCost()
  );

  //   клонируем объект и заполняем его
  let newCard = card.clone(true);
  newCard.find(".card-title").text(cardInformation.title);
  newCard.find(".card-text").text(cardInformation.text);
  newCard.find(".card__cost").text(cardInformation.cost + " $");

  //   добавляем в .cards
  card.parent().append(newCard);
}
