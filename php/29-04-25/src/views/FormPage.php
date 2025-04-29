<form id="myForm" action="/form/create" method="POST" onsubmit="return validateForm()">
    <h2>Форма обратной связи</h2>
    <?php if (!empty($successMessage)): ?>
        <div class="success"><?php echo htmlspecialchars($successMessage); ?></div>
    <?php endif; ?>
    <?php if (!empty($errors['name'])): ?>
        <div class="error"><?php echo htmlspecialchars($errors['name']); ?></div>
    <?php endif; ?>
    <label for="name">Имя:</label>
    <input type="text" id="name" name="name" value="<?php echo htmlspecialchars($_POST['name'] ?? ''); ?>">

    <?php if (!empty($errors['email'])): ?>
        <div class="error"><?php echo htmlspecialchars($errors['email']); ?></div>
    <?php endif; ?>
    <label for="email">Email:</label>
    <input type="email" id="email" name="email" value="<?php echo htmlspecialchars($_POST['email'] ?? ''); ?>">

    <?php if (!empty($errors['message'])): ?>
        <div class="error"><?php echo htmlspecialchars($errors['message']); ?></div>
    <?php endif; ?>
    <label for="message">Сообщение:</label>
    <textarea id="message" name="message" rows="4"><?php echo htmlspecialchars($_POST['message'] ?? ''); ?></textarea>

    <button type="submit">Отправить</button>
</form>
<script src="/scripts/validation.js"></script>