using System.Configuration;

try
{
    var arquivosAntigos = ConfigurationManager.AppSettings.Get("arquivos_antigos");
    var pontoGeral = ConfigurationManager.AppSettings.Get("ponto_geral");
    var horarioGeracaoArquivo = ConfigurationManager.AppSettings.Get("horario_geracao_arquivo");

    if (String.IsNullOrEmpty(arquivosAntigos))
        throw new Exception("Informe o diretório em arquivos_antigos no UnificadorRP.dll.config");
    if (String.IsNullOrEmpty(pontoGeral))
        throw new Exception("Informe o diretório em ponto_geral no UnificadorRP.dll.config");
    if (String.IsNullOrEmpty(horarioGeracaoArquivo))
        throw new Exception("Informe o horário que o arquivo vai ser gerado no UnificadorRP.dll.config");


    var diretorio = new DirectoryInfo(arquivosAntigos);
    var arquivos = diretorio.GetFiles("*.txt");

    if (arquivos.Count() == 0)
        throw new Exception("Não existem arquivos no diretório: " + arquivosAntigos);


    foreach (var arquivo in arquivos)
    {
        Console.WriteLine($"Lendo arquivo {arquivo.FullName}");
        string pontoGeralConteudo = File.ReadAllText(pontoGeral);
        string text = File.ReadAllText(arquivo.FullName);
        var sw = new StreamWriter(pontoGeral);
        Console.WriteLine("Concatenando...");
        sw.WriteLine(pontoGeralConteudo);
        sw.Write(text);
        sw.Close();

        Console.WriteLine($"Deletando arquivo {arquivo.FullName}");
        arquivo.Delete();

    }

    var agora = TimeSpan.Parse(DateTime.Now.ToShortTimeString());
    var fimDoDia = TimeSpan.Parse(horarioGeracaoArquivo);

    if (agora >= fimDoDia)
    {
        var ponto = pontoGeral;
        var split = ponto.Split(".");
        var arquivoNovo = split[0] + "_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "." + split[1];

        Console.WriteLine($"Gerando arquivo {arquivoNovo}");
        File.Copy(pontoGeral, arquivoNovo);

        Console.WriteLine($"Limpando arquivo {pontoGeral}");
        string pontoGeralConteudoNovo = File.ReadAllText(pontoGeral);
        var swNovo = new StreamWriter(pontoGeral);
        swNovo.Write("");
        swNovo.Close();

    }

    Console.WriteLine("Fim...");

    await Task.Delay(5000);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    await Task.Delay(5000);
}

