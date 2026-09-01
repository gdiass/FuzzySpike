using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] perguntas, candidatos;
            double[,] matrizFuzzy;
            PreencherTabelas2(out perguntas, out candidatos, out matrizFuzzy);

            int totalCandidatos = candidatos.Length;
            double[] probCandidatos = new double[totalCandidatos];

            // Distribuição inicial uniforme (1.0 / N)
            for (int c = 0; c < totalCandidatos; c++) probCandidatos[c] = 1.0 / totalCandidatos;

            var perguntasFeitas = new HashSet<int>();

            // Primeira pergunta via algoritmo dinâmico
            int proximaPergunta = FuzzyEngine.ObterMelhorPerguntaDinamica(matrizFuzzy, perguntas, probCandidatos, perguntasFeitas);

            Console.WriteLine($"\n Melhor Pergunta Selecionada: {perguntas[proximaPergunta]}\n");

            // Input do usuário
            string inputUsuario = Console.ReadLine();
            if (!double.TryParse(inputUsuario, out double respostaUsuario))
            {
                throw new ArgumentException("O valor digitado não é um número válido.");
            }

            //respostaUsuario = 0.75;
            Console.WriteLine($"#Input do Usuário: {respostaUsuario}");
            perguntasFeitas.Add(proximaPergunta);

            // Atualiza a distribuição de probabilidades dos candidatos
            FuzzyEngine.AtualizarProbabilidades(matrizFuzzy, proximaPergunta, respostaUsuario, probCandidatos);

            // Exibe probabilidades atualizadas
            for (int c = 0; c < totalCandidatos; c++)
            {
                Console.WriteLine($"{candidatos[c]}: {probCandidatos[c]:P1}");
            }

            for (int i = 0; i < 6; i++) // Exemplo: fazer 2 perguntas adicionais
            {
                NextQuestion(perguntas, candidatos, matrizFuzzy, totalCandidatos, probCandidatos, perguntasFeitas, out proximaPergunta, out inputUsuario, out respostaUsuario);
            }

            var resultado = candidatos.OrderByDescending(x => probCandidatos[Array.IndexOf(candidatos, x)]).First();
            Console.WriteLine($"\n !!!!!!!! O resultado é !!!!!!!!!!!!!!");
            Console.WriteLine($"\n {resultado}");

            inputUsuario = Console.ReadLine();
        }

        private static void PreencherTabelas(out string[] perguntas, out string[] candidatos, out double[,] matrizFuzzy)
        {
            perguntas = new[] {
                "É brinquedo de 'menino'?",
                "É de brincar em grupo?",
                "É para brincar ao ar livre?",
                "Pode molhar?"
            };
            candidatos = new [] { "Baralho", "Bola", "Boneca", "Pelúcia", "Quebra-cabeça" };

            // Matriz [Linhas = Perguntas, Colunas = Candidatos]
            matrizFuzzy = new double[,] {
                { 0.50, 1.00, 0.00, 0.25, 0.50 }, // Q0
                { 1.00, 0.75, 0.50, 0.50, 0.25 }, // Q1
                { 0.25, 1.00, 0.50, 0.00, 0.00 }, // Q2
                { 0.00, 1.00, 0.75, 0.00, 0.00 }  // Q3
            };
        }

        private static void PreencherTabelas2(out string[] perguntas, out string[] candidatos, out double[,] matrizFuzzy)
        {
            perguntas = new [] {
                "É uma pessoa real?",                                               // Q0
                "É do sexo feminino?",                                              // Q1
                "Nasceu no território do Brasil?",                                  // Q2
                "Foi um Presidente da República?",                                  // Q3
                "Foi um Monarca, Imperador ou membro da Realeza?",                  // Q4
                "Tem forte ligação com o período Colonial (antes de 1822)?",        // Q5
                "Tem forte ligação com o período Imperial (1822 a 1889)?",          // Q6
                "É reconhecido como líder militar ou lutou em guerras/revoltas?",   // Q7
                "Sofreu execução, assassinato ou teve morte trágica?",              // Q8
                "É historicamente reconhecido(a) como negro(a) ou pardo(a)?",       // Q9
                "É um(a) intelectual, escritor(a) ou cientista?",                   // Q10
                "É famoso(a) por sua atuação nas artes ou arquitetura?",            // Q11
                "Participou ativamente do processo de Independência do Brasil?",    // Q12
                "Esteve diretamente envolvido(a) na abolição da escravatura?",      // Q13
                "Usava coroa, chapéu militar ou acessório de cabeça marcante?",     // Q14
                "É frequentemente retratado(a) com barba ou bigode marcante?",      // Q15
                "Liderou quilombos ou revoltas populares/indígenas?",               // Q16
                "Viveu a maior parte de sua vida no século XX (após 1900)?",        // Q17
                "Assinou leis ou constituições históricas?",                        // Q18
                "Possui um feriado nacional diretamente ligado à sua história?"     // Q19
            };

            candidatos = new [] {
                "Dom Pedro I",          // C0
                "Dom Pedro II",         // C1
                "Princesa Isabel",      // C2
                "Zumbi dos Palmares",   // C3
                "Tiradentes",           // C4
                "Getúlio Vargas",       // C5
                "Juscelino Kubitschek", // C6
                "Machado de Assis",     // C7
                "Monteiro Lobato",      // C8
                "Oscar Niemeyer",       // C9
                "Maria Quitéria",       // C10
                "Anita Garibaldi",      // C11
                "Dona Leopoldina",      // C12
                "José Bonifácio",       // C13
                "Aleijadinho",          // C14
                "Chica da Silva",       // C15
                "Marechal Deodoro",     // C16
                "Frei Caneca",          // C17
                "Pedro Álvares Cabral", // C18
                "Dom João VI"           // C19
            };

            // Matriz [Linhas = Perguntas, Colunas = Candidatos]
            // 1.00 = Definitivamente sim | 0.75 = Muito provável / Parcialmente | 0.50 = Ambiguidade | 0.25 = Pouco provável | 0.00 = Definitivamente não
            matrizFuzzy = new double[,] {
                { 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00 }, // Q0: Pessoa real?
                { 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 1.00, 1.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 0.00 }, // Q1: Feminino?
                { 0.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 0.00, 1.00, 1.00, 1.00, 1.00, 1.00, 0.00, 0.00 }, // Q2: Nasceu no Brasil? (Pedro I, Leopoldina, Cabral, João VI nasceram na Europa)
                { 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00 }, // Q3: Presidente?
                { 1.00, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00 }, // Q4: Monarca/Realeza?
                { 0.25, 0.00, 0.00, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.25, 0.50, 1.00, 1.00, 0.00, 0.50, 1.00, 1.00 }, // Q5: Colonial?
                { 1.00, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.75, 0.00, 0.00, 1.00, 1.00, 1.00, 1.00, 0.00, 0.00, 0.75, 0.25, 0.00, 0.00 }, // Q6: Imperial?
                { 0.75, 0.00, 0.00, 1.00, 0.75, 0.50, 0.00, 0.00, 0.00, 0.00, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.75, 0.75, 0.00 }, // Q7: Militar/Líder/Guerra?
                { 0.00, 0.00, 0.00, 1.00, 1.00, 1.00, 0.25, 0.00, 0.00, 0.00, 0.00, 0.75, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00 }, // Q8: Morte trágica/Assassinato? (Zumbi, Tiradentes, Vargas, Caneca)
                { 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00 }, // Q9: Negro(a)/Pardo(a)?
                { 0.00, 0.75, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 0.25, 0.00, 0.00 }, // Q10: Intelectual/Escritor?
                { 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00 }, // Q11: Artes/Arquitetura? (Niemeyer, Aleijadinho)
                { 1.00, 0.00, 0.00, 0.00, 0.50, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00 }, // Q12: Independência do Brasil?
                { 0.00, 0.50, 1.00, 0.00, 0.00, 0.00, 0.00, 0.50, 0.00, 0.00, 0.00, 0.00, 0.00, 0.50, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00 }, // Q13: Abolição da escravatura?
                { 0.75, 0.75, 0.75, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.75, 0.00, 0.75, 0.00, 0.00, 0.00, 1.00, 0.00, 0.50, 0.75 }, // Q14: Coroa ou chapéu marcante?
                { 0.75, 1.00, 0.00, 0.00, 1.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 1.00, 0.00 }, // Q15: Barba/bigode marcante?
                { 0.00, 0.00, 0.00, 1.00, 0.25, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.75, 0.00, 0.00 }, // Q16: Liderou Quilombos/Revoltas locais? (Zumbi, Caneca)
                { 0.00, 0.00, 0.25, 0.00, 0.00, 1.00, 1.00, 0.25, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00 }, // Q17: Viveu no século XX (1900+)?
                { 1.00, 0.00, 1.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00 }, // Q18: Assinou leis históricas/constituições? (Pedro I, Isabel, Vargas, Deodoro)
                { 1.00, 0.00, 0.00, 1.00, 1.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 1.00, 0.00, 0.00, 0.00 }  // Q19: Possui um feriado associado? (Tiradentes, Zumbi, Pedro I, Deodoro)
            };
        }

        private static void NextQuestion(string[] perguntas, string[] candidatos, double[,] matrizFuzzy, int totalCandidatos, double[] probCandidatos, HashSet<int> perguntasFeitas, out int proximaPergunta, out string inputUsuario, out double respostaUsuario)
        {
            proximaPergunta = FuzzyEngine.ObterMelhorPerguntaPorVariancia(matrizFuzzy, perguntas, perguntasFeitas, probCandidatos);
            Console.WriteLine($"\nPróxima melhor pergunta selecionada: {perguntas[proximaPergunta]} \n");


            // Input do usuário
            inputUsuario = Console.ReadLine();
            if (!double.TryParse(inputUsuario, out respostaUsuario))
            {
                throw new ArgumentException("O valor digitado não é um número válido.");
            }

            //respostaUsuario = 0.25;
            Console.WriteLine($"#Input do Usuário: {respostaUsuario}");

            perguntasFeitas.Add(proximaPergunta);

            // Atualiza a distribuição de probabilidades dos candidatos
            FuzzyEngine.AtualizarProbabilidades(matrizFuzzy, proximaPergunta, respostaUsuario, probCandidatos);

            // Exibe probabilidades atualizadas
            for (int c = 0; c < totalCandidatos; c++)
            {
                Console.WriteLine($"{candidatos[c]}: {probCandidatos[c]:P1}");
            }
        }
    }
}
