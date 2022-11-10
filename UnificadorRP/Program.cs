using System.Configuration;

try
{
    var arquivosAntigos = ConfigurationManager.AppSettings.Get("arquivos_antigos");
    var pontoGeral = ConfigurationManager.AppSettings.Get("ponto_geral");

    if (String.IsNullOrEmpty(arquivosAntigos) || String.IsNullOrEmpty(pontoGeral))
        throw new Exception("Falta preencher o diretório");

    var diretorio = new DirectoryInfo(arquivosAntigos);
    var arquivos = diretorio.GetFiles("*.txt");

    if (arquivos.Count() == 0)
        throw new Exception("Não existem arquivos no diretório: " + arquivosAntigos);

    foreach (var arquivo in arquivos)
    {
        Console.WriteLine("Lendo arquivos...");
        string pontoGeralConteudo = File.ReadAllText(pontoGeral);
        string text = File.ReadAllText(arquivo.FullName);
        var sw = new StreamWriter(pontoGeral);
        Console.WriteLine("Concatenando arquivos...");
        sw.WriteLine(pontoGeralConteudo);
        sw.Write(text);
        sw.Close();
    }

    var ponto = pontoGeral;
    var split = ponto.Split(".");
    var arquivoNovo = split[0] + "_" + DateTime.Now.AddDays(1).Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "." + split[1];

    Console.WriteLine("Copiando arquivo...");
    File.Copy(pontoGeral, arquivoNovo);

    string pontoGeralConteudoNovo = File.ReadAllText(pontoGeral);
    var swNovo = new StreamWriter(pontoGeral);
    Console.WriteLine("Concatenando arquivos...");
    swNovo.Write("");
    swNovo.Close();

    Console.WriteLine("Fim...");

    await Task.Delay(5000);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

