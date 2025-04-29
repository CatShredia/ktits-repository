<?php
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $titleOfCity = htmlspecialchars($_POST["titleOfCity"]);
    $numberOfCity = htmlspecialchars($_POST["numberOfCity"]);

    if (empty($titleOfCity) || empty($numberOfCity)) {
        $errorMessage = "Вы ввели не все значения!";
    } else if ($numberOfCity >= 2025) {
        $errorMessage = "Дата основания слишком большая";
    } else {
        $errorMessage = "";

        echo "<p>Городу: " . $titleOfCity . " исполнилось " . (date("Y") - $numberOfCity) . " лет </p>";
    }
}
?>
<section>
    <h2 class="section-title" style="text-align: center;">Form city</h2>

    <form action="<?php ?>" class="cityForm" method="POST">
        <label for="titleOfCity">Название города</label>
        <input type="text" name="titleOfCity" id="titleOfCityInput" class="titleOfCityInput">
        <label for="numberOfCity">Дата основания города</label>
        <input type="number" name="numberOfCity" id="numberOfCityInput" class="numberOfCityInput">

        <button type="submit">OK</button>

        <br>

        <span><?php if (isset($errorMessage)) {
                    echo $errorMessage;
                } ?></span>
    </form>
</section>