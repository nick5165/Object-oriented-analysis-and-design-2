using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;

namespace SalesSystem.Core.Services;

public class DocumentGenerator
{
    public DocumentGenerator()
    {
        // Установка бесплатной лицензии для учебных целей
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public void GenerateDocument(string fullText, string outputPath)
    {
        // Создаем PDF документ через Fluent API
        Document.Create(container =>
        {
            // Начинаем описание страницы
            container.Page(page =>
            {
                // Настройки страницы
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                
                // Шрифт по умолчанию
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.TimesNewRoman));

                // Контент (основное тело документа)
                page.Content().Column(column =>
                {
                    string[] lines = fullText.Split('\n');
                    bool isFirstLine = true;

                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        // Оформление заголовка
                        if (isFirstLine)
                        {
                            column.Item().PaddingBottom(10).Text(line).SemiBold().FontSize(18).AlignCenter();
                            isFirstLine = false;
                            continue;
                        }

                        // Оформление подразделов
                        if (line.Contains("РАЗДЕЛ") || line.Contains("ДОГОВОР") || line.Contains("СПЕЦИФИКАЦИЯ"))
                        {
                            column.Item().PaddingTop(10).PaddingBottom(5).Text(line).Bold().FontSize(13);
                            continue;
                        }

                        // Оформление таблицы (строки с вертикальной чертой)
                        if (line.StartsWith("|"))
                        {
                            column.Item().Text(line).FontFamily(Fonts.CourierNew).FontSize(9);
                            continue;
                        }

                        // Оформление подписей и печатей
                        if (line.Contains("ЭЦП") || line.Contains("Подпись") || line.Contains("М.П."))
                        {
                            column.Item().PaddingTop(5).Text(line).Italic().FontSize(10).FontColor(Colors.Grey.Medium);
                            continue;
                        }

                        // Обычный текст
                        column.Item().Text(line);
                    }
                });

                // Подвал страницы
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Документ сформирован системой ЭДО. Страница ");
                    x.CurrentPageNumber();
                });
            });
        })
        .GeneratePdf(outputPath);
    }
}