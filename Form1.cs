
namespace Cambios
{
    using Cambios.Modelos;
    using Cambios.Servicos;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    public partial class Form1 : Form
    {
        #region Atributos

        private List<Rates> Rate;

        NetworkService networkService;

        private ApiService apiService;

        private DialogService dialogService;

        private DataService dataService;

        #endregion

        public Form1()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService();
            LoadRates();
        }


        //Deixar o metodo em async para a aplicação continuar a correr enquanto carrega as taxas
        private async void LoadRates()
        {
            bool load;


            labelResultado.Text = "A atualizar taxas....";

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {

                LoadLocalRates();
                load = false;
            }
            else
            {
                await LoadApiRates();
                load = true;
            }

            if (Rate.Count == 0)
            {
                labelResultado.Text = "Não há ligação à Internet " +
                    "e não foram préviamente carregadas as taxas " +
                    "Tente mais tarde";

                labelStatus.Text = "A primeira inicialização deverá ter ligação à internet";

                return;
            }


            //Preencher a comboBox da origem
            comboBoxOrigem.DataSource = Rate;
            comboBoxOrigem.DisplayMember = "Name";


            //Para não deixar ambas as Combos conectadas para poder selecionar itens diferentes
            comboBoxDestino.BindingContext = new BindingContext();

            //Preencher a comboBox do destino
            comboBoxDestino.DataSource = Rate;
            comboBoxOrigem.DisplayMember = "Name";


            labelResultado.Text = "Taxas atualizadas";

            //Caso a API seja carregada
            if (load)
            {
                labelStatus.Text = string.Format("Taxas carregadas da internet em {0:F}", DateTime.Now);
            }
            else
            {
                labelStatus.Text = string.Format("Taxas carregadas da Base de Dados.");
            }

            //Quando finalizar de carregar a API, vai estar preenchida a barra
            progressBar1.Value = 100;

            btnConverter.Enabled = true;

            btnTrocar.Enabled = true;
        }

        private void LoadLocalRates()
        {
            Rate = dataService.GetData();
        }

        private async Task LoadApiRates()
        {
            progressBar1.Value = 0;

            var response = await apiService.GetRates("http://rates.somee.com", "/api/rates");

            Rate = (List<Rates>)response.Result;

            dataService.DeleteData();

            dataService.SaveData(Rate);
        }

        private void btnConverter_Click(object sender, EventArgs e)
        {
            Converte();
        }

        private void Converte()
        {
            if (string.IsNullOrEmpty(TextBoxValor.Text))
            {
                dialogService.ShowMessage("Erro", "Insira um valor a converter");
                return;
            }

            decimal valor;
            if (!decimal.TryParse(TextBoxValor.Text, out valor))
            {
                dialogService.ShowMessage("Erro de conversão", "Valor terá que ser numérico");
                return;
            }

            if (comboBoxOrigem.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem de escolher uma moeda a converter");
                return;
            }

            if (comboBoxDestino.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem de escolher uma moeda de destino para converter");
                return;
            }

            var taxaOrigem = (Rates)comboBoxOrigem.SelectedItem;

            var taxaDestino = (Rates)comboBoxDestino.SelectedItem;

            var valorConvertido = valor / (decimal)taxaOrigem.TaxRate * (decimal)taxaDestino.TaxRate;

            labelResultado.Text = string.Format("{0} {1:C2} = {2} {3:C2}", taxaOrigem.Code,
                valor,
                taxaDestino.Code,
                valorConvertido);

        }

        private void btnTrocar_Click(object sender, EventArgs e)
        {
            Troca();
        }

        private void Troca()
        {
            var aux = comboBoxOrigem.SelectedItem;
            comboBoxOrigem.SelectedItem = comboBoxDestino.SelectedItem;
            comboBoxDestino.SelectedItem = aux;
            Converte();
        }
    }
}
