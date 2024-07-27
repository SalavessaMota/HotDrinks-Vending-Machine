namespace WinForms_Avaliacao_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btn_caf.Focus();                            // Coloca o foco no bot�o do caf� ao iniciar o formul�rio
        }

        // Declara��o dos pre�os das bebidas como constantes
        const int precoCaf = 25;
        const int precoCap = 30;
        const int precoCho = 35;
        const int precoCha = 20;

        string bebida = "";                                  // Variavel que ir� conter o nome da bebida escolhida

        int[] valores = { 200, 100, 50, 20, 10, 5 };    // Valores das moedas

        int[] bufferMoedas = { 0, 0, 0, 0, 0, 0 };      // Array que contem as quantias de moedas introduzidas pelo utilizador, antes das mesmas passarem ao moedeiro da m�quina, sendo cada indice correspondente a um tipo de moeda

        int[] moedeiro = { 0, 0, 0, 0, 0, 0 };          // Array que contem as quantias de moedas presentes no moedeiro da m�quina, inicializei a 0's para efeito de testes :)

        System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.somMaquina);      // Ficheiro de som que o programa ir� utilizar quando feita uma compra de bebida com sucesso
        

        private void zeraArray(int[] array) // m�todo para por a 0 todos os elementos de um array
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
        }

        private int totalArray(int[] array) // m�todo que retorna o valor (dinheiro) total de um array de quantidades de moedas
        {
            int total = 0;
            for(int i = 0; i < array.Length; i++)
            {
                total += array[i] * valores[i];
            }
            return total;
        }

        // Atualiza a informa��o do total inserido na textbox
        private void ApresentaInfo()
        {
            txt_Info.Text = $"Total inserido: {(totalArray(bufferMoedas)/100.00).ToString("C")} ";
        }

        // Reinicia o sistema para uma nova utiliza��o
        private void btn_iniciar_Click(object sender, EventArgs e)
        {
            zeraArray(bufferMoedas);
            txt_Info.Text = "";
            btn_caf.Enabled = true;
            btn_cap.Enabled = true;
            btn_cho.Enabled = true;
            btn_cha.Enabled = true;
            btn_5c.Enabled = true;
            btn_10c.Enabled = true;
            btn_20c.Enabled = true;
            btn_50c.Enabled = true;
            btn_1e.Enabled = true;
            btn_2e.Enabled = true;
        }

        // Adiciona informa��es relevantes ao escolher uma bebida
        private void AdicionaInfoBebida(string bebida, int preco, int numMensagem)
        {
            if (numMensagem == 0)
            {
                txt_Info.Text += "Ainda n�o introduziu nenhuma quantia!\r\n";
                DesativaBotoes();           // Desativa os bot�es ap�s a tentativa de compra
            }
            else if (numMensagem == 1)
            {
                txt_Info.Text += "\r\n\r\nAinda n�o introduziu a quantia suficiente!\r\nA devolver moedas introduzidas...\r\n";
                zeraArray(bufferMoedas);    // devolve as moedas introduzidas sem as colocar no moedeiro
                DesativaBotoes();           // Desativa os bot�es ap�s a tentativa de compra
            }
            else
            {
                player.Play();
                txt_Info.Text += "\r\n\r\nDisfrute do seu " + bebida + " :)";
                DesativaBotoes(); // Desativa os bot�es ap�s a compra
            }
        }

        // Calcula o troco e exibe as informa��es na textbox
        private void CalculaTrocos(string bebida, int precoBebida)
        {
            /* 
             * 
             * Neste m�todo vamos utilizar um array temporario (clone do moedeiro), para que possamos analisar se � poss�vel realizar a opera��o pretendida garantindo troco devido ao utilizador,
             * se tal for poss�vel, actualizamos a informa��o constante do moedeiro da m�quina e tamb�m o array "buffer" das moedas introduzidas. 
             * Caso contrario, n�o "mexemos" no array moedeiro
             * 
             */


            int[] moedas = { 0, 0, 0, 0, 0, 0 }; // Quantidades de cada moedas, cada indice corresponde a um tipo de moeda (2euros,1euro,....)
            string[] valNomes = { "2 �", "1 �", "0.50 �", "0.20 �", "0.10 �", "0.05 �" }; // Array de string com "os nomes" das moedas para exibi��o

            int[] moedeiroTemp = (int[])moedeiro.Clone();       // Cria��o do array temporario
            int troco, totalInserido;

            totalInserido = totalArray(bufferMoedas);           // Calculo do total inserido atrav�s do array das moedas inseridas
            troco = totalInserido - precoBebida;
           
            // Itera sobre as moedas e calcula quantas de cada s�o necess�rias
            for (int i = 0; i < moedeiro.Length; i++)
            {
                while (troco >= valores[i] && (moedeiroTemp[i] > 0 || bufferMoedas[i] > 0))     // Enquanto houver troco a devolver e houver moedas tanto no moedeiro clone quanto nas moedas inseridas
                {
                    moedas[i]++;
                    if (bufferMoedas[i] > 0)        // primeiro vai ver �s moedas inseridas
                    {
                        bufferMoedas[i]--;
                    }
                    else if (moedeiroTemp[i] > 0)   // e s� depois vai ao moedeiro
                    {
                        moedeiroTemp[i]--;
                    }
                    else
                    {
                        break;
                    }
                    troco -= valores[i];
                }
            }
            if(troco != 0 && totalInserido > precoBebida)       // verificar se foi possivel acumular moedas suficientes para dar troco
            {
                txt_Info.Text += $"\r\n\r\nSem troco. Por favor introduza o montante exacto.\r\nA devolver moedas...\r\n";
                DesativaBotoes(); // Desativa os bot�es ap�s a tentativa de compra
                zeraArray(bufferMoedas);
            }
            else if (totalInserido == 0)                        // verificar se houve realmente introdu��o de moedas para mostrar a informa�ao correcta na textbox
            {
                AdicionaInfoBebida(bebida, precoBebida, 0);
                zeraArray(bufferMoedas);
            }

            else if (totalInserido < precoBebida)               // verificar se o montante introduzido � suficiente para a bebida pretendida, para ent�o mostrar a informa�ao correcta na textbox
            {
                AdicionaInfoBebida(bebida, precoBebida, 1);
                zeraArray(bufferMoedas);
            }
            else
            {
                Array.Copy(moedeiroTemp, moedeiro, moedeiro.Length);    // se "tudo correu bem", vamos actualizar o moedeiro "real"
                AdicionaInfoBebida(bebida, precoBebida, 2);
                
                txt_Info.Text += $"\r\n\r\nTem a receber: {((totalInserido - precoBebida) / 100.00).ToString("C")}\r\n\r\n";


                // ciclo para mostrar, a cada intera��o com a m�quina, o array de quantidades de moedas no moedeiro
                for (int i = 0;i < moedeiro.Length; i++)
                {
                    moedeiro[i] = moedeiro[i] + bufferMoedas[i];
                    if (moedas[i] > 0)
                        txt_Info.Text += $"{moedas[i]} moedas de {valNomes[i]}\r\n";
                }
            }

            // Apresenta��o das quantidades de moedas no moedeiro apenas para efeito de controle/teste
            txt_Info.Text += "\r\n(Array do moedeiro:";
            for (int i = 0;i < moedeiro.Length; i++)
            {
                if (i < moedeiro.Length - 1)
                    txt_Info.Text += " " + moedeiro[i] + " -";
                else
                    txt_Info.Text += " " + moedeiro[i];
            }
            txt_Info.Text += ")";
        }

        // Desativa todos os bot�es (usado ap�s a compra de uma bebida)
        private void DesativaBotoes()
        {
            btn_caf.Enabled = false;
            btn_cap.Enabled = false;
            btn_cho.Enabled = false;
            btn_cha.Enabled = false;
            btn_5c.Enabled = false;
            btn_10c.Enabled = false;
            btn_20c.Enabled = false;
            btn_50c.Enabled = false;
            btn_1e.Enabled = false;
            btn_2e.Enabled = false;
        }

        #region BotoesInserirMoedas
        // Eventos para cada bot�o de inserir moedas
        // Cada evento adiciona 1 moeda ao buffer das moedas introduzidas e apresenta/actualiza a informa�ao na textbox
        private void btn_5c_Click(object sender, EventArgs e)
        {
            bufferMoedas[5]++;
            ApresentaInfo();
        }

        private void btn_10c_Click(object sender, EventArgs e)
        {
            bufferMoedas[4]++;
            ApresentaInfo();
        }

        private void btn_20c_Click(object sender, EventArgs e)
        {
            bufferMoedas[3]++;
            ApresentaInfo();
        }

        private void btn_50c_Click(object sender, EventArgs e)
        {
            bufferMoedas[2]++;
            ApresentaInfo();
        }

        private void btn_1e_Click(object sender, EventArgs e)
        {
            bufferMoedas[1]++;
            ApresentaInfo();
        }

        private void btn_2e_Click(object sender, EventArgs e)
        {
            bufferMoedas[0]++;
            ApresentaInfo();
        }

        #endregion

        #region BotoesEscolhaBebidas
        // Eventos para cada bot�o de escolha de bebidas
        // Cada evento define a bebida escolhida, calcula os trocos necess�rios e adiciona a informa��o relevante � textbox
        private void btn_caf_Click(object sender, EventArgs e)
        {
            bebida = "Caf�";
            CalculaTrocos(bebida, precoCaf);
        }

        private void btn_cap_Click(object sender, EventArgs e)
        {
            bebida = "Cappuccino";
            CalculaTrocos(bebida, precoCap);
        }

        private void btn_cho_Click(object sender, EventArgs e)
        {
            bebida = "Chocolate";
            CalculaTrocos(bebida, precoCho);
        }

        private void btn_cha_Click(object sender, EventArgs e)
        {
            bebida = "Ch�";
            CalculaTrocos(bebida, precoCha);
        }

        #endregion
    }
}
