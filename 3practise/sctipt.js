// TODO: 1 задача
function task1() {
	let coustThing = parseInt(prompt("Введите стоимость товара"));
	let money = parseInt(prompt("Введите количество денег у клиента"));

	if (coustThing == money) {
		alert("Покупка совершена");
	} else if (coustThing > money) {
		alert(`Для покупки не хватает ${coustThing - money} р.`);
	} else if (coustThing < money) {
		alert(`Сдача ${money - coustThing} р.`);
	}
}
// TODO: 3 задача
// let result;
// if (a + b < 4) {
//   result = 'Мало';
// } else {
//   result = 'Много';
// }

function task3() {
	// let result;
	let a = parseInt(prompt("Введите a"));
	let b = parseInt(prompt("Введите b"));

	let result = a + b < 4 ? "Мало" : "Много";

	alert(result);
}
// TODO: 4 задача
// let message;
// if (login == 'Сотрудник') {
//   message = 'Привет';
// } else if (login == 'Директор') {
//   message = 'Здравствуйте';
// } else if (login == '') {
//   message = 'Нет логина';
// } else {
//   message = ''Пусто”;
// }

function task4() {
    let login = prompt("Введите логин")

	let message =
		login == "Сотрудник"
			? "Привет"
			: login == "Директор"
			? "Здравствуйте"
			: login == ""
			? "Нет логина"
			: "Пусто";

    alert(message)
}

task4();
