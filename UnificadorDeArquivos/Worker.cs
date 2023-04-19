namespace UnificadorDeArquivos
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IConfiguration _configuration { get; }

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tempoDeEspera = _configuration.GetSection("TempoDeEspera").Value;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker rodando em: {time}", DateTime.Now);

                try
                {
                    var arquivosAntigos = _configuration.GetSection("ArquivosAntigos").Value;
                    var pontoGeral = _configuration.GetSection("ArquivoProntoGeral").Value;
                    var horarioGeracaoArquivo = _configuration.GetSection("HorarioGeracaoArquivoDestino").Value;

                    if (String.IsNullOrEmpty(arquivosAntigos))
                        throw new Exception("Informe o diretório em arquivos_antigos no appsettings.json");
                    if (String.IsNullOrEmpty(pontoGeral))
                        throw new Exception("Informe o diretório em ponto_geral no appsettings.json");
                    if (String.IsNullOrEmpty(horarioGeracaoArquivo))
                        throw new Exception("Informe o horário que o arquivo vai ser gerado no appsettings.json");

                    var diretorio = new DirectoryInfo(arquivosAntigos);
                    var arquivos = diretorio.GetFiles("*.txt");

                    if (arquivos.Count() == 0)
                        throw new Exception("Não existem arquivos no diretório: " + arquivosAntigos);

                    foreach (var arquivo in arquivos)
                    {
                        _logger.LogInformation($"Lendo arquivo {arquivo.FullName}");
                        string pontoGeralConteudo = File.ReadAllText(pontoGeral);
                        string text = File.ReadAllText(arquivo.FullName);
                        var sw = new StreamWriter(pontoGeral);
                        _logger.LogInformation("Concatenando...");
                        sw.WriteLine(pontoGeralConteudo);
                        sw.Write(text);
                        sw.Close();

                        _logger.LogInformation($"Deletando arquivo {arquivo.FullName}");
                        arquivo.Delete();
                    }

                    var agora = TimeSpan.Parse(DateTime.Now.ToShortTimeString()).Ticks;
                    var fimDoDia = TimeSpan.Parse(horarioGeracaoArquivo).Ticks;

                    if (agora >= fimDoDia)
                    {
                        var ponto = pontoGeral;
                        var split = ponto.Split(".");
                        var arquivoNovo = $"{split[0]}_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.{split[1]}";

                        _logger.LogInformation($"Gerando arquivo {arquivoNovo}");
                        File.Copy(pontoGeral, arquivoNovo);

                        _logger.LogInformation($"Limpando arquivo {pontoGeral}");
                        string pontoGeralConteudoNovo = File.ReadAllText(pontoGeral);
                        var swNovo = new StreamWriter(pontoGeral);
                        swNovo.Write("");
                        swNovo.Close();
                    }
                    await Task.Delay(TimeSpan.FromHours(Convert.ToInt16(tempoDeEspera)));
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e.Message);
                    await Task.Delay(TimeSpan.FromHours(Convert.ToInt16(tempoDeEspera)));
                }
            }
        }
    }
}