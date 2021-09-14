# Бот Finodays Bank

Для запуска требуется установка [.Net Core Runtime](https://dotnet.microsoft.com/download/dotnet/5.0/runtime)

Запускать программы требуется в следующем порядке:
1. SentimentAnalysis.API
2. SentimentAnalysis.Bot

Для взаимодействия с нейронной сетью можно использовать:
- Консольный проект SentimentAnalysis.Test
- Телеграмм бота [@SentimentAssay_Bot](https://t.me/SentimentAssay_Bot)
- Rest Api запросы описаны в [Swagger](http://localhost:5000/swagger/index.html)

### Консольное приложение

В консольном приложении реализованны следующие команды:
1. -p,--predicted ["Text"] - Команда для определения тональности сообщения
2. -e,--evaluate - Команда для получения характеристик тренеровки модели
3. -t,--train - Команда для запуска тренировки

### Телеграмм бот

Отправленные сообщения в чате будут сразу проанализированны ботом.
В телеграм боте реализованны следующие команды:
1. /help - Выдаёт список доступных команд
2. /Evaluate -Команда для получения характеристик тренеровки модели
3. /ModelTraining - Команда для запуска тренировки