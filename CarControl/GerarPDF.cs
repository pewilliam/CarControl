using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CarControl.Models;

namespace CarControl
{
    internal class GerarPDF
    {
        public void GerarRelatorio(List<Modelo> modeloList, int qtdModelos)
        {
            var modelosSelecionados = modeloList.Take(qtdModelos).ToList();
            if (modelosSelecionados.Count > 0)
            {
                var pxPorMm = 72 / 25.2f;
                var pdf = new Document(iTextSharp.text.PageSize.A4, 15 * pxPorMm, 15 * pxPorMm, 15 * pxPorMm, 20 * pxPorMm);
                var nomeArquivo = $"modelos-{DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss")}.pdf";
                var arquivo = new FileStream(nomeArquivo, FileMode.Create);
                var write = iTextSharp.text.pdf.PdfWriter.GetInstance(pdf, arquivo);
                pdf.Open();

                var fontbase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

                var fonteparagrafo = new iTextSharp.text.Font(fontbase, 32, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                var titulo = new Paragraph("Relatório de modelos\n\n", fonteparagrafo);
                titulo.Alignment = Element.ALIGN_CENTER;
                pdf.Add(titulo);

                var tabela = new PdfPTable(5);
                tabela.DefaultCell.BorderWidth = 0;
                tabela.WidthPercentage = 100;

                CriarCelula(tabela, "IdModelo", PdfPCell.ALIGN_CENTER, true);
                CriarCelula(tabela, "Nome", PdfPCell.ALIGN_LEFT, true);
                CriarCelula(tabela, "Cor", PdfPCell.ALIGN_CENTER, true);
                CriarCelula(tabela, "Placa", PdfPCell.ALIGN_CENTER, true);
                CriarCelula(tabela, "Preço", PdfPCell.ALIGN_CENTER, true);

                foreach (var m in modeloList)
                {
                    CriarCelula(tabela, m.IdModelo.ToString(), PdfPCell.ALIGN_CENTER);
                    CriarCelula(tabela, m.Nome);
                    CriarCelula(tabela, m.Cor, PdfPCell.ALIGN_CENTER);
                    CriarCelula(tabela, m.Placa.ToString(), PdfPCell.ALIGN_CENTER);
                    CriarCelula(tabela, m.Preco.ToString(), PdfPCell.ALIGN_CENTER);
                }

                pdf.Add(tabela);

                pdf.Close();
                arquivo.Close();

                var caminhopdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivo);

                if (File.Exists(caminhopdf))
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        Arguments = $"/c start {caminhopdf}",
                        FileName = "cmd.exe",
                        CreateNoWindow = true
                    });
                }


            }
        }

        static void CriarCelula(PdfPTable tabela, string texto,
            int alinhamento = PdfPCell.ALIGN_LEFT,
            bool negrito = false, bool italico = false,
            int tamanhoFonte = 12, int alturaCelula = 25)
        {
            int estilo = iTextSharp.text.Font.NORMAL;
            if (negrito && italico)
            {
                estilo = iTextSharp.text.Font.BOLDITALIC;
            }
            else if (negrito)
            {
                estilo = iTextSharp.text.Font.BOLD;
            }
            else if (italico)
            {
                estilo = iTextSharp.text.Font.ITALIC;
            }

            BaseFont fonteBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            iTextSharp.text.Font fonte = new iTextSharp.text.Font(fonteBase, tamanhoFonte,
                estilo, iTextSharp.text.BaseColor.Black);

            //cor de fundo diferente para linhas pares e ímpares
            var bgColor = iTextSharp.text.BaseColor.White;
            if (tabela.Rows.Count % 2 == 1)
                bgColor = new BaseColor(0.95f, 0.95f, 0.95f);

            PdfPCell celula = new PdfPCell(new Phrase(texto, fonte));
            celula.HorizontalAlignment = alinhamento;
            celula.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            celula.Border = 0;
            celula.BorderWidthBottom = 1;
            celula.PaddingBottom = 5; //pra alinhar melhor verticalmente
            celula.FixedHeight = alturaCelula;
            celula.BackgroundColor = bgColor;
            tabela.AddCell(celula);
        }
    }
}
