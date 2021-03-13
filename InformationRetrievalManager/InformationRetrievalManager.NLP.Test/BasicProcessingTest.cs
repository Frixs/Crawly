using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace InformationRetrievalManager.NLP.Test
{
    /// <summary>
    /// Test <see cref="BasicProcessing"/>
    /// </summary>
    public class BasicProcessingTest : IDisposable
    {
        #region Private Members

        private readonly ITestOutputHelper _testOutputHelper;
        private BasicProcessing _processing;

        #endregion

        #region Constructor

        /// <summary>
        /// Do "global" initialization here; Called before every test method.
        /// </summary>
        public BasicProcessingTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            //_processing = new BasicProcessing(new Tokenizer(), new Stemmer(), null, false, true, true);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Do "global" teardown here; Called after every test method.
        /// </summary>
        public void Dispose()
        {
            _processing = null;
        }

        #endregion

        #region Tests

        /// <summary>
        /// Basic test
        /// </summary>
        [Fact]
        public void Index_ContainsKey()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, false, true, true);

            string text = "Æauík";

            #endregion

            #region Act

            _processing.Index(text);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            Assert.True(wordFrequencies.ContainsKey("cau"));

            #endregion
        }

        /// <summary>
        /// Check HTML tags
        /// </summary>
        [Fact]
        public void Index_CheckHtml()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(), null, false, true, true);

            string text = "<li>";

            #endregion

            #region Act

            _processing.Index(text);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            Assert.True(wordFrequencies.ContainsKey(text));

            #endregion
        }

        /// <summary>
        /// Check links
        /// </summary>
        [Fact]
        public void Index_CheckLink()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(), null, false, true, true);

            string text = "http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10355101 link";

            #endregion

            #region Act

            _processing.Index(text);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            Assert.False(wordFrequencies.ContainsKey("http"));
            Assert.True(wordFrequencies.ContainsKey("http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10355101"));

            #endregion
        }

        /// <summary>
        /// Check tokenization
        /// </summary>
        [Fact]
        public void Index_CheckTokenization()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(), null, false, false, false);

            string text1 = "(pìstí).";
            string text2 = "1280x800";
            string text3 = "pr*sata";

            #endregion

            #region Act

            _processing.Index(text1);
            _processing.Index(text2);
            _processing.Index(text3);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("pìstí")));
            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("1280x800")));
            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("pr*sata")));

            #endregion
        }

        /// <summary>
        /// Check stop words
        /// </summary>
        [Fact]
        public void Index_CheckStopWords()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), new StopWordRemover(ProcessingLanguage.CZ), true, false, false);

            string text1 = "Tímto textem britskı The Guardian Jana Èulíka nepotìšil";
            string text2 = "Jestlie nepøijde, mìl by se omluvit.";
            string text3 = "Šedesát procent naší elektøiny vyrábíme z dováeného plynu, pøièem prùmìr EU je okolo 40 %.";
            string text4 = "Tento seznam mùe bıt revidován po dvou letech, pøièem samotné programy mohou bıt pøedloeny jednou za rok.";
            string text5 = "Tímto poznáním pøekroèuje enské hnutí hranice feminismu a stává se hnutím pokrokovım, demokratickım. Doznávám, e jsem byl znaènì pøekvapen tímto novım dùkazem praktické stránky soudruhovıch theorií. Èechové byli stíeni tímté osudem po staletích hrdinského odboje proti nìmeckému útlaku. Tímto zpùsobem také, kdy opìt chcete zaèít pracovat, aktivní spoøiè obrazovky ukonèujete. Tím jste urèili, e program vyhledá všechny soubory, jejich název tímto slovem zaèíná. Patnáct procent s tímto názorem nesouhlasilo a pìtina muù nevìdìla, jak odpovìdìt. Celou tu pohádku o tøech Garridebech si patrnì vymyslel právì za tímto úèelem. Vloíte znaménko rovná se kadı vzorec musí tímto znaménkem zaèínat. Zpøedu kráèel Stivín, za nim jel rytíø mladı a za tímto klusal starec. Pøed tímto gentlemanem mùete øíci vše, co byste hodlal svìøit mnì. Všeobecnì dluno uznati, e tímto nejvıhodnìším místem je škola. Chtìl bych Vás proto tímto dopisem poprosit o odpovìï na tyto otázky. Vím to a ráda bych, abyste i vy zaèal svou práci s tímto vìdomím. Tehdejší prezident Charles King musí pod tímto tlakem odstoupit. Pocítila jsem ostré bodnutí árlivosti nad tímto cizincem. Páe, pohledem tímto pøekvapen, po celém tìle se tøásl. Tímto emailem bych se chtìla informovat o pøíznacích AIDS. Hlava mi témìø vybuchla snahou uvaovat tímto zpùsobem. Nechala jsem tímto temnım zjištìním naplnit své oèi. Ètvrtina respondentù naopak s tímto názorem souhlasila. Rance zdál se bıti ponìkud pozloben tímto odboèením. Tímto strašlivım zpùsobem jsem nabyl svoje dìdictví. Vìèná škoda, e tímto umìním plıtvala na Dolpha. Nepøipouštìl jsem si tímto smìrem jedinou myšlenku. Dodavatel tímto informuje odbìratele v souladu s zák. Víš, e mi nikdo neposlal kvìtiny tímto zpùsobem. Ne, rozhodnì se nesnese s tímto morbidním panákem. Tímto veèerem se poèalo podivínství kapitána J. Mohla bıt rána zasazena tímto pøedmìtem? Mohla. Jasnovidec byl zøejmì polichocen tímto uznáním. ";
            string text6 = "Aèkoli se celı rok uèil, známky na vysvìdèení má podprùmìrné.";

            #endregion

            #region Act

            _processing.Index(text1);
            _processing.Index(text2);
            _processing.Index(text3);
            _processing.Index(text4);
            _processing.Index(text5);
            _processing.Index(text6);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("tímté"))); // tímto = stopword - expl. for IR ex.
            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("aèkoli")));
            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("jestlie")));
            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("pøièem")));

            #endregion
        }

        /// <summary>
        /// Check date
        /// </summary>
        [Fact]
        public void Index_CheckDate()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(), new StopWordRemover(), false, false, false);

            string text1 = "11.2. 2015";
            string text2 = "15.5.2010";

            #endregion

            #region Act

            _processing.Index(text1);
            _processing.Index(text2);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            Assert.True(wordFrequencies.ContainsKey("11.2."));
            Assert.True(wordFrequencies.ContainsKey("15.5.2010"));

            #endregion
        }

        /// <summary>
        /// Check diacritics
        /// </summary>
        [Fact]
        public void Index_CheckDiacritics()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, false, true, false);

            string text1 = "æau";
            string text2 = "cau";
            string text3 = "caú";
            string text4 = "cáu";
            string text5 = "èau";

            #endregion

            #region Act

            _processing.Index(text1);
            _processing.Index(text2);
            _processing.Index(text3);
            _processing.Index(text4);
            _processing.Index(text5);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            var pText = _processing.ProcessWord("èau");
            if (wordFrequencies.ContainsKey(pText))
                Assert.Equal(5, wordFrequencies[pText]);
            else
                Assert.True(false);

            #endregion
        }

        /// <summary>
        /// Check lower case
        /// </summary>
        [Fact]
        public void Index_CheckLowerCase()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, true, false, false);

            string text1 = "BOMB";
            string text2 = "Bomba";
            string text3 = "bomba";
            string text4 = "BOMBY";

            #endregion

            #region Act

            _processing.Index(text1);
            _processing.Index(text2);
            _processing.Index(text3);
            _processing.Index(text4);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            var pText = _processing.ProcessWord("bomb");
            if (wordFrequencies.ContainsKey(pText))
                Assert.Equal(4, wordFrequencies[pText]);
            else
                Assert.True(false);

            #endregion
        }

        /// <summary>
        /// Check stemming
        /// </summary>
        [Fact]
        public void Index_CheckStemming()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, false, false, false);

            string text1 = "smìjí";
            string text2 = "smìju";
            string text3 = "smìješ";
            string text4 = "smìjeme";
            string text5 = "smìjí";
            string text6 = "smìjou";

            #endregion

            #region Act

            _processing.Index(text1);
            _processing.Index(text2);
            _processing.Index(text3);
            _processing.Index(text4);
            _processing.Index(text5);
            _processing.Index(text6);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            var pText1 = _processing.ProcessWord("smìjí");
            if (wordFrequencies.ContainsKey(pText1))
                Assert.Equal(4, wordFrequencies[pText1]);
            else
                Assert.True(false);

            var pText2 = _processing.ProcessWord("smìješ");
            if (wordFrequencies.ContainsKey(pText2))
                Assert.Equal(1, wordFrequencies[pText2]);
            else
                Assert.True(false);

            var pText3 = _processing.ProcessWord("smìjeme");
            if (wordFrequencies.ContainsKey(pText3))
                Assert.Equal(1, wordFrequencies[pText3]);
            else
                Assert.True(false);

            #endregion
        }

        /// <summary>
        /// Check lower case and stemming
        /// </summary>
        [Fact]
        public void Index_CheckLowerCaseAndStemming()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, true, false, false);

            string text1 = "BOMB";
            string text2 = "Bomba";
            string text3 = "bomba";
            string text4 = "bomby";

            #endregion

            #region Act

            _processing.Index(text1);
            _processing.Index(text2);
            _processing.Index(text3);
            _processing.Index(text4);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            var pText = _processing.ProcessWord("bomb");
            if (wordFrequencies.ContainsKey(pText))
                Assert.Equal(4, wordFrequencies[pText]);
            else
                Assert.True(false);

            #endregion
        }

        /// <summary>
        /// Check long text
        /// </summary>
        [Fact]
        public void Index_CheckLongText()
        {
            #region Arrange

            _processing = new BasicProcessing(new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, true, false, false);

            string text = "èáú jak se máš?" +
                "<li> o co jsti se snail a jak,</li>\n" +
                "<li> jakım vısledkem skonèila vaše akce,</li>\n" +
                "<li> v kolik hodin jste akci provádìli,</li>\n" +
                "studentskı prùkaz je jiska\n" +
                " Sooperméïa. A pøesnì v souladu s názvem. Poolka filmu je opravdu mrtvá. To jsou ty momenty, kdy se metrosexooální narcis Gájen Gejnolds prochází po pøehlídkovém moloo a snaí se vám oorpootnì a za kadou cenu narvat nagelovanou rookoje katany mezi poolky. Tehdy dostáváte kıèovitou, místy a debilnì felální trapárnoo pro retardy, kombinovanou s dost soochım a noodnım dìjem a zbyteènì sympatickım záporákem. Ale pokood jste tøeba nesmírnì serióznì beroucí se, sebestøednı, slizkı, plešatìjící emoteplouš jménem Robert, urèitì si i tak SMÍCHY naprskáte flitry do popkornoo a samım vzroošením rozmaete øasenkoo. Naštìstí je tu ale ta druhá poolka, a ta je opravdoo vıivná! To jsou ty momenty, kdy se Gájen nasouká do pro nìj pøíhodného èerveného latexoo a stane se z nìj cynickı hláškoojící zmrd, kterı to všem tìm otylım ètenáøoom omalovánek napálí bez servítkoo pøímo do xichtoo. Tehdy dostáváte koolantní a krvavou akcièkoo, napìchovanou brootálními bonbonmoty, trefnım popkooltoorním laškováním, poniováním Xmenù a dìláním kokota z Volverína. Škoda jen, e toohle ooroveò neoodreli celıch sto minut, ale jinak po Kikásu a Strácích galaxie tøetí vlaštovka, která ukazooje, e kdy se leporela pro dyslektiky pøestanou brát mentorsky a vánì a necílí na dvanáctileté mentální mootanty èi morálnì neposqrnìné šosácké zroody, mooe z toho bıt solidní taškaøice. A moc rád bych v boodoucnu vidìl homixovı krosouvr Deadpool vs. Kapitán Homokokot: Oosvit oplzlosti.(20.2.2016)\n" +
                "všechny komentáøe uivatele / [Trvalı odkaz: http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10355101]\n" +
                "koukáte na to? ****\n" +
                "Deadpool mì pøíjemnì pøekvapil. Ne post/pubertálním humorem, ten jsem spíš pøeil. Ani ne roztomilou sebeuvìdomìlostí procesu vyprávìní, neb ta patøí k základní narativní vıbavì souèasné zábavné fikce. S èím však Deadpool pracuje vskutku dùmyslnì, je vıstavba osnovy jeho vyprávìní. Èasová pásma, odboèky, situace - staví na nenápadném variování úèelnì omezeného mnoství vzorcù, take i pøes snahu diváka neustále pøekvapovat nakonec nepùsobí nestøídmì. Jo, na povrchu jde o romantickı pøíbìh na pozadí sebestøedné komiksové parodie, ale vıstavbou je to vlastnì stará dobrá detektivka drsné školy. V lecèem klasiètìjší ne mnohé z tìch, které se k této tradici v posledních dekádách hlásí.(11.2.2016)\n" +
                "všechny komentáøe uivatele / [Trvalı odkaz: http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10358805]\n" +
                "sákrıš ty jsou tak nekorektní a to bolí... ****\n" +
                "Koukat se na Deadpoola ,to je trochu jako jít po Václaváku s oralım kámošem, kterı si stoupne uprostøed chodníku a zaène chcát. Kadıch deset vteøin zaøve: BACHA MÁM V RUCE PTÁKA!, co je nejdøív docela funny, ale pak se to stane krapet pøedvídatelné a únavné. Deadpool je odlišnı ne tím, e by se tolik od ostatních superheroes lišil, ale tím, e odlišnost neustále tematizuje a sype divákùm do ksichtu. Jinak je úplnì stejnì prùhlednı jako CapAm, jen tam, kde se kapitán chová jako Dušín, se Deadpool nutnì zachová vdy jako kokot. Je to prostì modelovı návrat potlaèeného. Marvel tak dlouho odsouval násilí, vulgaritu a sex ze svıch filmù, a vzniklo dost materiálu na Deadpoola, kterı ukázkovì zaplòuje všechny díry (pìstí). Funguje to jako objednávkovı fan service a lubrikant pro další X-Meny, kde se urèitì nebude klít a masturbovat. A stejnì tak pro celé marvelovské univerzum, a u za ním stojí kdokoli. Neberte to špatnì - hlášky jsou bezva, akce fajn. Ale pod pozlátkem frkù vo korektnosti a honìní péra je v jádru úplnì stejnì jalová romantická story se špatnım padouchem (Ed Skrein = lame), jako v pøípadì mnoha dalších komiksákù. Deadpool vydìlává na tom, e na svoje slabiny poukáe, ale vısledkem není tak zábavná a soudrná podívaná jako Stráci galaxie, ale spíš zmatenì klièkující pøešívka, která svoje slabiny maskuje pubertálními vıstøelky. Jen to z mého pohledu nefunguje jako film, ale spíš jako fanboyovská smìsice gagù s promìnlivou úrovní. S rostoucí stopáí roste i pocit, e film jede na automat a na jeden dobrı gag vychrlí tøí prùmìrné. Take OK, beru, ale okouzlení Kick-Assem se neopakuje, Vaughn troti dovede chcát proti vìtru i bez toho, aby vám stokrát zdùraznil, e pøi tom drí v ruce ptáka a to se nedìlá. Škoda, e mi není o 20 let míò. Jak správnì poznamenal kolega Samohan Øepák: byl by to nejlepší film, co jsem kdy vidìl. Tohle musím v tradici èeského filmu pøekøtít na SUPERHRDINSKI FILM. [60%]\n" +
                "Nerikam, ze mi hlaskovaci postava Deadpoola nebo ksicht Ryana Reynoldse sedl uplne dokonale, ani ze mi vsech 1000 vtipku prislo vtipnych, ale tohle je v dobe kdy se masy hrnou do kin na plochy, nenapadity, okoukany, najisto vydelecny akcni mrdky, nenatocis pomalu nic drzyho a nekrestanskyho za poradny prachy, komiksovej boom kolikrat uz sakra nudi... tak tohle je proste skvela kapka zivy vody. A ja mam velkou radost, ze to zvalcuje pokladny kin po cely planete (s vyjimkou tam kde to neprojde pres cenzuru), protoze to je jednoznacnej signal, ze chceme tocit i filmy pro dospely, jazykem co nas mluvi vetsina a s ujetym humorem, co ma kurva beztak tolik z vas. Jo, chceme i NEKOREKTNI filmy pro lidi co nejsou nudny upjaty pici.\n" +
                "Z malého/bezvıznamného štìku ve Wolverinovi rovnou do první ligy? Tohle srazilo všechny odborníky (pøes kvalitu - kritiky, ano :) - i trby) do kolen. Nevìøil jsem, e ten film mùe fungovat. Obavy z roztøíštìnosti filmu do jednotlivıch vtipnıch scén se ale nenaplnily a to halvnì díky skvìle zvolené struktuøe vyprávìní. Deadpool ale hlavnì láká diváky na specifickı humor (kdo sledoval internet, sociální sítì a plakáty okolo Deadpoola, ten ví...) a tady nabírá film opravdu na obrátkách. Je tu hromada naráek na jiné filmy, na Wolverina, na Xmeny, na casting filmù... a pak je tu tuna sexistickıch vtipù, které pobaví vìtšinou nejvìtší pr*sata v sále (a jejich partnerky, co se smìjí také všemu:)). Kdy nad tím zpìtnì pøemıšlím, vìtšina tìchto \"Im touching myself tonight\" vtipù je vlatnì zbyteèná a film by fungoval i bez nich (a jejich kvalita je neuvìøitelnì rùznorodá). Seèteno podtreno, tohle je jiná komiksárna, sprostá, odváná a procenty hodnì nadhodnocená, ale i pøesto zábavná! PPS: je to vlastnì správnì, aby si film uvìdomoval sám sebe a kritizoval okolní universa nebo obsazení rolí?:) To u je na jinou diskuzi... PS: jsem sám zvìdav, co ještì se z prvního Wolverina \"odøízne\" vzhledem k tomu, e Deadpool má svùj restart, Gambita èeká to samé...(15.2.2016)\n" +
                "všechny komentáøe uivatele / [Trvalı odkaz: http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10360720]\n" +
                "to je BOMBA ****\n" +
                "Po prvním shlédnutí traileru jsem Deadpoolovi moc nevìøil, ale nakonec musím øíct, e je to fakticky bomba. Nicménì zdejší hodnocení mì vyslovenì vytáhlo do kina, abych se nakonec sám pøesvìdèil. Vısledek je takovej, e Deadpool pøesnì splnil, pro co byl urèenej. První chvíle sice byly dost rozpaèitı. Po pùl hodinì jsem nic moc nevìdìl, co si o filmu myslet, ale jakmile Deadpool rozjel trojboj v hláškované, nemìl konkurenci a sypal to ze sebe jak bábovièky písek. V tu chvíli jsem si uíval naprosto boí naráky na všechny monı a nemonı postavy superhrdinského univerza a pøemıšlel nad tím, jestli takovej film nìkdo za dvacet, tøicet let ocení. Ve vısledku je to stejnì ale jedno, protoe trby se tvoøí teï a tady a ty v tuhle chvíli mluví za vše." +
                "<li> jméno, pøíjmení, orion login, studentské èíslo.</li>" +
                "Tablet PC - Intel Atom Quad Core Z3735F, kapacitní multidotykovı IPS 10.1\" LED 1280x800, Intel HD Graphics, RAM 2GB, 64GB eMMC, WiFi, Bluetooth 4.0, webkamera 2 Mpx + 5 Mpx, 2èlánková baterie, Windows 10 Home 32bit + MS Office Mobile";

            #endregion

            #region Act

            _processing.Index(text);
            var wordFrequencies = _processing.WordFrequencies;
            PrintWordFrequencies(wordFrequencies);

            #endregion

            #region Assert

            var pText1 = _processing.ProcessWord("bomb");
            if (wordFrequencies.ContainsKey(pText1))
                Assert.Equal(2, wordFrequencies[pText1]);
            else
                Assert.True(false);

            var pText2 = _processing.ProcessWord("trby");
            if (wordFrequencies.ContainsKey(pText2))
                Assert.Equal(2, wordFrequencies[pText2]);
            else
                Assert.True(false);

            var pText3 = _processing.ProcessWord("z3735f");
            if (wordFrequencies.ContainsKey(pText3))
                Assert.Equal(1, wordFrequencies[pText3]);
            else
                Assert.True(false);

            var pText4 = _processing.ProcessWord("</li>");
            if (wordFrequencies.ContainsKey(pText4))
                Assert.Equal(4, wordFrequencies[pText4]);
            else
                Assert.True(false);

            #endregion
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Print word frequencies
        /// </summary>
        /// <param name="wordFrequencies">The word frequencies</param>
        private void PrintWordFrequencies(IReadOnlyDictionary<string, int> wordFrequencies)
        {
            foreach (KeyValuePair<string, int> entry in wordFrequencies)
                _testOutputHelper.WriteLine(entry.Key + ":\t" + entry.Value);
            PrintSortedDictionary(wordFrequencies);
        }

        /// <summary>
        /// Print sorted words from the lsit of frequencies
        /// </summary>
        /// <param name="wordFrequencies">The list of frequencies</param>
        private void PrintSortedDictionary(IReadOnlyDictionary<string, int> wordFrequencies)
        {
            string[] words = wordFrequencies.Keys.ToArray();
            Array.Sort(words);
            _testOutputHelper.WriteLine(string.Join(", ", words));
        }

        #endregion
    }
}
