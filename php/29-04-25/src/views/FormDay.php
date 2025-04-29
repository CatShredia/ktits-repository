<?php
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $time = htmlspecialchars($_POST["time"]);
    $message = "";

    if (empty($time)) {
        $errorMessage = "Вы не ввели значение!";
    } else {
        $errorMessage = "";

        echo $time;

        $time_parts = explode(":", $time);
        if (count($time_parts) == 2) {
            $hour = (int)$time_parts[0];
            $minute = (int)$time_parts[1];

            if ($hour >= 12 && $hour < 18) {
                $message = "день";
            } elseif ($hour >= 18 || $hour < 6) {
                $message = "вечер";
            } else {
                $message = "утро";
            }
            echo "<p>Сообщение: " . $message . "</p>";
        } else {
            $message = "Неверный формат времени.";
        }
    }
}
?>
<section>
    <h2 class="section-title" style="text-align: center;">Form city</h2>

    <form action="<?php ?>" class="cityForm" method="POST">
        <label for="time">Время</label>
        <input type="time" name="time" id="timeInput" class="timeInput">

        <button type="submit">OK</button>

        <br>

        <span><?php if (isset($errorMessage)) {
                    echo $errorMessage;
                } ?></span>
    </form>

    <h3>Сейчас: <?php echo date('H:i:s') ?></h3>
</section>