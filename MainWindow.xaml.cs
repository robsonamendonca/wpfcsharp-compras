using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Cosmos;
using MessageBox = System.Windows.MessageBox;

namespace wpfcompras
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int COLUNAS = 40;
        private decimal totalCompra = 0;
        public MainWindow()
        {
            InitializeComponent();
            preparar_inicio();
            TravaCampos();
            imgMercadoPago.Visibility = Visibility.Hidden;
        }

        

        private void btnAbrir_Click(object sender, RoutedEventArgs e)
        {
            TravaCampos();
            imgMercadoPago.Visibility = Visibility.Hidden;
            DateTime data = DateTime.UtcNow;
            CultureInfo ci = CultureInfo.InvariantCulture;
            string dataF = data.ToString("dd/MM/yyyy", ci);
            string horaF = data.ToString("hh:mm:ss.F", ci);
            lstRecibo.Items.Clear();

            string texto = string.Concat(Enumerable.Repeat("-", COLUNAS));
            lstRecibo.Items.Add(texto);
            texto = formataComEspaco("COMUNIDADE TORNE-SE UM PROGRAMADOR");
            lstRecibo.Items.Add(texto);
            texto = string.Concat(Enumerable.Repeat("-", COLUNAS));
            lstRecibo.Items.Add(texto);
            texto = formataComEspaco("SEU ENDERECO DA LOJA,XX CIDADE / UF ");
            lstRecibo.Items.Add(texto);
            texto = "CNPJ: 00.000.000/00";
            lstRecibo.Items.Add(texto);
            texto = "INSCR. EST.: 000.000.00 ";
            lstRecibo.Items.Add(texto);
            texto = formataComEspaco(" C U P O M   F I S C A L ");
            lstRecibo.Items.Add(texto);
            texto = formataComEspaco($"{dataF}  {horaF}  XYZ.XYZ  BR");
            lstRecibo.Items.Add(texto);
            texto = string.Concat(Enumerable.Repeat("-", COLUNAS));
            lstRecibo.Items.Add(texto);
            texto = formataComEspaco("PRODUTO");
            lstRecibo.Items.Add(texto);
            texto = formataComEspaco("   QTDE    UNIT.    PREÇO TOTAL");
            lstRecibo.Items.Add(texto);
            texto = string.Concat(Enumerable.Repeat("-", COLUNAS));
            lstRecibo.Items.Add(texto);

            btnAbrir.IsEnabled = false;
            btnFechar.IsEnabled = true;
            btnIncluir.IsEnabled = true;
            txtProduto.Focus();

        }

        private string formataComEspaco(string texto)
        {
            string tmp = Left(texto, COLUNAS);
            int tamanho = tmp.Length;
            decimal replica = COLUNAS - tamanho;
            int space = Convert.ToInt32(Math.Truncate(replica / 2));
            return string.Concat(Enumerable.Repeat(" ", space)) + tmp + string.Concat(Enumerable.Repeat(" ", space)) ;

        }

        private string Left(string texto, int tamanho)
        {
            if (texto.Length < tamanho)
                return texto;
            else
                return texto.Substring(0, tamanho);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            string formapagamento = "Dinheiro";
            decimal dinheiro = 0;
            decimal troco = 0;

            MessageBoxResult result = MessageBox.Show("Deseja finalizar a compra e: \n\n Pagar a conta via Dinheiro 'Sim' ou \n 'Não' vou pagar via Mercado Pago?\n\nPoderá cancelar para voltar a comprar!",
  "Confirmação", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var valor = new InputBox("TOTAL COMPRA: " + String.Format("{0:C}", totalCompra) + "\nValor Recebido:", "Receber o Dinheiro!", "Arial", 12).ShowDialog();
                dinheiro = Convert.ToDecimal(valor);
                troco = dinheiro - totalCompra;
                if (troco > 0)
                {
                    System.Windows.MessageBox.Show("Muito obrigado! Seu troco é: " + String.Format("{0:C}", troco));
                }
                TravaCampos();
            }
            else if (result == MessageBoxResult.No)
            {
                // Código para o botão No
                imgMercadoPago.Visibility = Visibility.Visible;
                System.Windows.MessageBox.Show("Por favor infomre Valor total da compra: " + String.Format("{0:C}", totalCompra) + "\n Solicite ao cliente scanner o QR CODE!");
                formapagamento = "MERCADO PAGO";
                TravaCampos();
            }
            else
            {
                // Código para o botão Cancel
                return;
            }

            preparar_inicio();
            var texto = string.Concat(Enumerable.Repeat("-", COLUNAS));
            lstRecibo.Items.Add(texto);
            texto = formataComEspaco($"TOTAL               {String.Format("{0:C}", totalCompra)}");
            lstRecibo.Items.Add(texto);
            // FALTA O TROCO E O DINHEIRO
            if (formapagamento == "Dinheiro")
            {
                texto = formataComEspaco($"DINHEIRO            {String.Format("{0:C}", dinheiro)}");
                lstRecibo.Items.Add(texto);
                texto = formataComEspaco($"TROCO               {String.Format("{0:C}", troco)}");
                lstRecibo.Items.Add(texto);
            }
            else
            {
                texto = formataComEspaco($"MERCADO PAGO        {String.Format("{0:C}", totalCompra)}");
                lstRecibo.Items.Add(texto);
            }

            texto = string.Concat(Enumerable.Repeat("-", COLUNAS));
            lstRecibo.Items.Add(texto);

            texto = formataComEspaco($"OBRIGADO PELA PREFERENCIA!!!");
            lstRecibo.Items.Add(texto);
            texto = formataComEspaco($"BORA PARA AULA!!! PRATICAR!!!");
            lstRecibo.Items.Add(texto);
            texto = string.Concat(Enumerable.Repeat("-", COLUNAS));
            lstRecibo.Items.Add(texto);
            lblSubTotal.Content = "R$ 0.00";
            totalCompra = 0;
        }

        //LostFocus=="txtProduto_LostFocus"
        private void txtProduto_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtProduto.Text))
            {
                
                try
                {
                    var dados = Cosmos.GetProdutoApi(txtProduto.Text);
                    if ( dados != null)
                    {
                        var lista = dados?.Split('|');
                        if (lista.Length > 0)
                        {
                            txtProduto.Text = lista[1];
                            txtPreco.Text = lista[0];
                            txtQtd.Focus();
                            imgFotoProduto.Source = new BitmapImage(new Uri(lista[2]));
                        }
                    }
                    else
                    {
                        imgFotoProduto.Source = null;
                    }
                }
                catch (Exception ex)
                {
                    //sem dados da api
                    imgFotoProduto.Source = null;
                    Console.WriteLine(ex.ToString());
                }
                
                
            }
        }

        private void txtIncluir_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidaCampos())
            {
                return;
            }
            int qtd = Convert.ToInt32(txtQtd.Text);
            decimal preco = Convert.ToDecimal(txtPreco.Text);
            decimal total = qtd * preco;
            var texto = formataComEspaco(txtProduto.Text.ToUpper());
            lstRecibo.Items.Add(texto);
            texto = formataComEspaco($" {txtQtd.Text}  {String.Format("{0:C}", preco)}   {String.Format("{0:C}", total)}");
            lstRecibo.Items.Add(texto);
            totalCompra += total;
            lblSubTotal.Content = $"{String.Format("{0:C}", totalCompra)}";
            limparItem();
        }

        private void limparItem()
        {
            txtPreco.Text = "";
            txtQtd.Text = "";
            txtProduto.Text = "";
            txtProduto.Focus();
        }

        private void TravaCampos()
        {
            txtPreco.IsEnabled = !txtPreco.IsEnabled;
            txtQtd.IsEnabled = !txtQtd.IsEnabled;
            txtProduto.IsEnabled = !txtProduto.IsEnabled;
        }

        public void preparar_inicio()
        {
            btnFechar.IsEnabled = false;
            btnAbrir.IsEnabled = true;
            btnIncluir.IsEnabled = false;
            limparItem();
            if (btnAbrir.IsEnabled)
            {
                btnAbrir.Focus();
            }
        }
        public Boolean ValidaCampos()
        {
            int valorInteiro;
            double valorDouble;

            //Validando se é inteiro utilizando o recurso TryParse
            //No método passamos primeiro uma string, e depois a saída "out"
            bool isNumeroInteiro = int.TryParse(txtQtd.Text, out valorInteiro);
            bool isNumeroDouble = double.TryParse(txtPreco.Text, out valorDouble);


            if (string.IsNullOrWhiteSpace(txtProduto.Text))
            {
                System.Windows.MessageBox.Show("Por favor informe um nome de produto antes de continuar!");
                txtProduto.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtQtd.Text))
            {
                System.Windows.MessageBox.Show("Por favor informe a quantidade do produto antes de continuar!");
                txtQtd.Focus();
                return false;
            }
            else
            {
                if (!isNumeroInteiro)
                {
                    System.Windows.MessageBox.Show("Por favor informe somente números na quantidade do produto antes de continuar!");
                    txtQtd.Focus();
                    return false;
                }   
            }
            if (string.IsNullOrWhiteSpace(txtPreco.Text))
            {
                System.Windows.MessageBox.Show("Por favor informe o preço do produto antes de continuar!");
                txtPreco.Focus();
                return false;
            }
            else
            {
                if(!isNumeroInteiro && !isNumeroDouble)
                {
                    System.Windows.MessageBox.Show("Por favor informe somente número no preço do produto antes de continuar!");
                    txtPreco.Focus();
                    return false;
                }
            }
            return true;
        }
    }
}
