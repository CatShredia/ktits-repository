function task1() {
	let a = parseInt(prompt("Введите a"));
	let b = parseInt(prompt("Введите b"));

	for (let i = a; i < b; i++) {
		if (i % 2 == 0) {
			alert(i);
		}
	}
}
function task2() {
	let i = 0;
	while (i < 3) {
		alert(`number ${i}!`);
		i++;
	}
}
function task3() {
	while (true) {
		number = parseInt(prompt("Введите число, большее 10"));
		if (number > 10) {
			break;
		}
	}
}
function minAB(a, b) {
    if (a < b) {
        return a
    } if (a > b) {
        return b
    } else {
        return null
    }
}
// task1()
// task2()
// task3()
alert(minAB(parseInt(prompt("Введите a")), parseInt(prompt("Введите b"))))
