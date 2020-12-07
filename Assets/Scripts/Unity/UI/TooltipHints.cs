public static class TooltipHints
{
    public const string TERRAIN_DIMENSIONS = "Número de vértices do terreno por linha e coluna.";
    public const string TERRAIN_SEED = "Utilizado como base para a função geradora de números aleatórios. \nA mesma semente sempre gerará o mesmo terreno.";
    public const string THERMAL_STRENGTH = "Controla a quantidade de sedimento que será movimentado. \nRecomenda-se manter um valor próximo a 0,5.";
    public const string THERMAL_TALUS = "Controla a partir de qual inclinação o terreno sofrerá os efeitos da erosão. \nQuanto maior o valor, maior a inclinação necessária.";
    public const string HYDRAULIC_RAIN = "Controla a quantidade de chuva sobre o terreno. Quanto maior, \nmais água será derramada na superfície do terreno.";
    public const string HYDRAULIC_SOLUBILITY = "Controla o fator de solubilidade da superfície. Quanto maior, \nmais sedimentos serão desprendidos e carregados pela água.";
    public const string HYDRAULIC_EVAPORATION = "Controla o fator de evaporação da água da chuva. Quanto maior, \nmaior a quantidade de água evaporada ao final de cada iteração.";
    public const string GPU_FLAG = "Se o processo será executado em GPU ou não. \nRecomenda-se manter sempre ativado.";
    public const string ITERATIONS = "Quantidade de repetições da operação. \nValores maiores que 100 não apresentam grandes mudanças.";
    public const string MAP_VISUALIZATION = "O mapa de altimetria demonstra as diferenças do terreno com base na altura, \nenquanto o mapa de declividade demonstra as diferenças com base na inclinação.";
}
