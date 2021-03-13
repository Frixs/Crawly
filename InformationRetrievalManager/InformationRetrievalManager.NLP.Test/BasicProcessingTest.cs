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

            string text = "�au�k";

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

            string text1 = "(p�st�).";
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

            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("p�st�")));
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

            string text1 = "T�mto textem britsk� The Guardian Jana �ul�ka nepot�il";
            string text2 = "Jestli�e nep�ijde, m�l by se omluvit.";
            string text3 = "�edes�t procent na�� elekt�iny vyr�b�me z dov�en�ho plynu, p�i�em� pr�m�r EU je okolo 40 %.";
            string text4 = "Tento seznam m��e b�t revidov�n po dvou letech, p�i�em� samotn� programy mohou b�t p�edlo�eny jednou za rok.";
            string text5 = "T�mto pozn�n�m p�ekro�uje �ensk� hnut� hranice feminismu a st�v� se hnut�m pokrokov�m, demokratick�m. Dozn�v�m, �e jsem byl zna�n� p�ekvapen t�mto nov�m d�kazem praktick� str�nky soudruhov�ch theori�. �echov� byli st�eni t�mt� osudem po stalet�ch hrdinsk�ho odboje proti n�meck�mu �tlaku. T�mto zp�sobem tak�, kdy� op�t chcete za��t pracovat, aktivn� spo�i� obrazovky ukon�ujete. T�m jste ur�ili, �e program vyhled� v�echny soubory, jejich� n�zev t�mto slovem za��n�. Patn�ct procent s t�mto n�zorem nesouhlasilo a p�tina mu�� nev�d�la, jak odpov�d�t. Celou tu poh�dku o t�ech Garridebech si patrn� vymyslel pr�v� za t�mto ��elem. Vlo��te znam�nko rovn� se ka�d� vzorec mus� t�mto znam�nkem za��nat. Zp�edu kr��el Stiv�n, za nim jel ryt�� mlad� a za t�mto klusal starec. P�ed t�mto gentlemanem m��ete ��ci v�e, co byste hodlal sv��it mn�. V�eobecn� dlu�no uznati, �e t�mto nejv�hodn��m m�stem je �kola. Cht�l bych V�s proto t�mto dopisem poprosit o odpov�� na tyto ot�zky. V�m to a r�da bych, abyste i vy za�al svou pr�ci s t�mto v�dom�m. Tehdej�� prezident Charles King mus� pod t�mto tlakem odstoupit. Poc�tila jsem ostr� bodnut� ��rlivosti nad t�mto cizincem. P�e, pohledem t�mto p�ekvapen, po cel�m t�le se t��sl. T�mto emailem bych se cht�la informovat o p��znac�ch AIDS. Hlava mi t�m�� vybuchla snahou uva�ovat t�mto zp�sobem. Nechala jsem t�mto temn�m zji�t�n�m naplnit sv� o�i. �tvrtina respondent� naopak s t�mto n�zorem souhlasila. Rance zd�l se b�ti pon�kud pozloben t�mto odbo�en�m. T�mto stra�liv�m zp�sobem jsem nabyl svoje d�dictv�. V��n� �koda, �e t�mto um�n�m pl�tvala na Dolpha. Nep�ipou�t�l jsem si t�mto sm�rem jedinou my�lenku. Dodavatel t�mto informuje odb�ratele v souladu s z�k. V�, �e mi nikdo neposlal kv�tiny t�mto zp�sobem. Ne, rozhodn� se nesnese s t�mto morbidn�m pan�kem. T�mto ve�erem se po�alo podiv�nstv� kapit�na J. Mohla b�t r�na zasazena t�mto p�edm�tem? Mohla. Jasnovidec byl z�ejm� polichocen t�mto uzn�n�m. ";
            string text6 = "A�koli se cel� rok u�il, zn�mky na vysv�d�en� m� podpr�m�rn�.";

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

            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("t�mt�"))); // t�mto = stopword - expl. for IR ex.
            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("a�koli")));
            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("jestli�e")));
            Assert.True(wordFrequencies.ContainsKey(_processing.ProcessWord("p�i�em�")));

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

            string text1 = "�au";
            string text2 = "cau";
            string text3 = "ca�";
            string text4 = "c�u";
            string text5 = "�au";

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

            var pText = _processing.ProcessWord("�au");
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

            string text1 = "sm�j�";
            string text2 = "sm�ju";
            string text3 = "sm�je�";
            string text4 = "sm�jeme";
            string text5 = "sm�j�";
            string text6 = "sm�jou";

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

            var pText1 = _processing.ProcessWord("sm�j�");
            if (wordFrequencies.ContainsKey(pText1))
                Assert.Equal(4, wordFrequencies[pText1]);
            else
                Assert.True(false);

            var pText2 = _processing.ProcessWord("sm�je�");
            if (wordFrequencies.ContainsKey(pText2))
                Assert.Equal(1, wordFrequencies[pText2]);
            else
                Assert.True(false);

            var pText3 = _processing.ProcessWord("sm�jeme");
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

            string text = "��� jak se m�?" +
                "<li> o co jsti se sna�il a jak,</li>\n" +
                "<li> jak�m v�sledkem skon�ila va�e akce,</li>\n" +
                "<li> v kolik hodin jste akci prov�d�li,</li>\n" +
                "studentsk� pr�kaz je jiska\n" +
                " Sooperm��a. A p�esn� v souladu s n�zvem. Poolka filmu je opravdu mrtv�. To jsou ty momenty, kdy se metrosexoo�ln� narcis G�jen Gejnolds proch�z� po p�ehl�dkov�m moloo a sna�� se v�m oorpootn� a za ka�dou cenu narvat nagelovanou rookoje� katany mezi poolky. Tehdy dost�v�te k��ovitou, m�sty a� debiln� fel�ln� trap�rnoo pro retardy, kombinovanou s dost sooch�m a� noodn�m d�jem a zbyte�n� sympatick�m z�por�kem. Ale pokood jste t�eba nesm�rn� seri�zn� berouc� se, sebest�edn�, slizk�, ple�at�j�c� emoteplou� jm�nem Robert, ur�it� si i tak SM�CHY naprsk�te flitry do popkornoo a sam�m vzroo�en�m rozma�ete �asenkoo. Na�t�st� je tu ale ta druh� poolka, a ta je opravdoo v��ivn�! To jsou ty momenty, kdy se G�jen nasouk� do pro n�j p��hodn�ho �erven�ho latexoo a stane se z n�j cynick� hl�kooj�c� zmrd, kter� to v�em t�m otyl�m �ten��oom omalov�nek nap�l� bez serv�tkoo p��mo do xichtoo. Tehdy dost�v�te koolantn� a krvavou akci�koo, nap�chovanou broot�ln�mi bonbonmoty, trefn�m popkooltoorn�m la�kov�n�m, poni�ov�n�m Xmen� a d�l�n�m kokota z Volver�na. �koda jen, �e toohle oorove� neoodr�eli cel�ch sto minut, ale jinak po Kik�su a Str�c�ch galaxie t�et� vla�tovka, kter� ukazooje, �e kdy� se leporela pro dyslektiky p�estanou br�t mentorsky a v�n� a nec�l� na dvan�ctilet� ment�ln� mootanty �i mor�ln� neposqrn�n� �os�ck� zroody, moo�e z toho b�t solidn� ta�ka�ice. A moc r�d bych v boodoucnu vid�l homixov� krosouvr Deadpool vs. Kapit�n Homokokot: Oosvit oplzlosti.(20.2.2016)\n" +
                "v�echny koment��e u�ivatele / [Trval� odkaz: http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10355101]\n" +
                "kouk�te na to? ****\n" +
                "Deadpool m� p��jemn� p�ekvapil. Ne post/pubert�ln�m humorem, ten jsem sp� p�e�il. Ani ne roztomilou sebeuv�dom�lost� procesu vypr�v�n�, neb ta pat�� k z�kladn� narativn� v�bav� sou�asn� z�bavn� fikce. S ��m v�ak Deadpool pracuje vskutku d�mysln�, je v�stavba osnovy jeho vypr�v�n�. �asov� p�sma, odbo�ky, situace - stav� na nen�padn�m variov�n� ��eln� omezen�ho mno�stv� vzorc�, tak�e i p�es snahu div�ka neust�le p�ekvapovat nakonec nep�sob� nest��dm�. Jo, na povrchu jde o romantick� p��b�h na pozad� sebest�edn� komiksov� parodie, ale v�stavbou je to vlastn� star� dobr� detektivka drsn� �koly. V lec�em klasi�t�j�� ne� mnoh� z t�ch, kter� se k t�to tradici v posledn�ch dek�d�ch hl�s�.(11.2.2016)\n" +
                "v�echny koment��e u�ivatele / [Trval� odkaz: http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10358805]\n" +
                "s�kr�� ty jsou tak nekorektn� a� to bol�... ****\n" +
                "Koukat se na Deadpoola ,to je trochu jako j�t po V�clav�ku s o�ral�m k�mo�em, kter� si stoupne uprost�ed chodn�ku a za�ne chc�t. Ka�d�ch deset vte�in za�ve: BACHA M�M V RUCE PT�KA!, co� je nejd��v docela funny, ale pak se to stane krapet p�edv�dateln� a �navn�. Deadpool je odli�n� ne t�m, �e by se tolik od ostatn�ch superheroes li�il, ale t�m, �e odli�nost neust�le tematizuje a sype div�k�m do ksichtu. Jinak je �pln� stejn� pr�hledn� jako CapAm, jen tam, kde se kapit�n chov� jako Du��n, se Deadpool nutn� zachov� v�dy jako kokot. Je to prost� modelov� n�vrat potla�en�ho. Marvel tak dlouho odsouval n�sil�, vulgaritu a sex ze sv�ch film�, a� vzniklo dost materi�lu na Deadpoola, kter� uk�zkov� zapl�uje v�echny d�ry (p�st�). Funguje to jako objedn�vkov� fan service a lubrikant pro dal�� X-Meny, kde se ur�it� nebude kl�t a masturbovat. A stejn� tak pro cel� marvelovsk� univerzum, a� u� za n�m stoj� kdokoli. Neberte to �patn� - hl�ky jsou bezva, akce fajn. Ale pod pozl�tkem frk� vo korektnosti a hon�n� p�ra je v j�dru �pln� stejn� jalov� romantick� story se �patn�m padouchem (Ed Skrein = lame), jako v p��pad� mnoha dal��ch komiks�k�. Deadpool vyd�l�v� na tom, �e na svoje slabiny pouk�e, ale v�sledkem nen� tak z�bavn� a soudr�n� pod�van� jako Str�ci galaxie, ale sp� zmaten� kli�kuj�c� p�e��vka, kter� svoje slabiny maskuje pubert�ln�mi v�st�elky. Jen to z m�ho pohledu nefunguje jako film, ale sp� jako fanboyovsk� sm�sice gag� s prom�nlivou �rovn�. S rostouc� stop�� roste i pocit, �e film jede na automat a na jeden dobr� gag vychrl� t�� pr�m�rn�. Tak�e OK, beru, ale okouzlen� Kick-Assem se neopakuje, Vaughn troti� dovede chc�t proti v�tru i bez toho, aby v�m stokr�t zd�raznil, �e p�i tom dr�� v ruce pt�ka a to se ned�l�. �koda, �e mi nen� o 20 let m��. Jak spr�vn� poznamenal kolega Samohan �ep�k: byl by to nejlep�� film, co jsem kdy vid�l. Tohle mus�m v tradici �esk�ho filmu p�ek�t�t na SUPERHRDINSKI FILM. [60%]\n" +
                "Nerikam, ze mi hlaskovaci postava Deadpoola nebo ksicht Ryana Reynoldse sedl uplne dokonale, ani ze mi vsech 1000 vtipku prislo vtipnych, ale tohle je v dobe kdy se masy hrnou do kin na plochy, nenapadity, okoukany, najisto vydelecny akcni mrdky, nenatocis pomalu nic drzyho a nekrestanskyho za poradny prachy, komiksovej boom kolikrat uz sakra nudi... tak tohle je proste skvela kapka zivy vody. A ja mam velkou radost, ze to zvalcuje pokladny kin po cely planete (s vyjimkou tam kde to neprojde pres cenzuru), protoze to je jednoznacnej signal, ze chceme tocit i filmy pro dospely, jazykem co nas mluvi vetsina a s ujetym humorem, co ma kurva beztak tolik z vas. Jo, chceme i NEKOREKTNI filmy pro lidi co nejsou nudny upjaty pici.\n" +
                "Z mal�ho/bezv�znamn�ho �t�ku ve Wolverinovi rovnou do prvn� ligy? Tohle srazilo v�echny odborn�ky (p�es kvalitu - kritiky, ano :) - i tr�by) do kolen. Nev��il jsem, �e ten film m��e fungovat. Obavy z rozt��t�nosti filmu do jednotliv�ch vtipn�ch sc�n se ale nenaplnily a to halvn� d�ky skv�le zvolen� struktu�e vypr�v�n�. Deadpool ale hlavn� l�k� div�ky na specifick� humor (kdo sledoval internet, soci�ln� s�t� a plak�ty okolo Deadpoola, ten v�...) a tady nab�r� film opravdu na obr�tk�ch. Je tu hromada nar�ek na jin� filmy, na Wolverina, na Xmeny, na casting film�... a pak je tu tuna sexistick�ch vtip�, kter� pobav� v�t�inou nejv�t�� pr*sata v s�le (a jejich partnerky, co se sm�j� tak� v�emu:)). Kdy� nad t�m zp�tn� p�em��l�m, v�t�ina t�chto \"Im touching myself tonight\" vtip� je vlatn� zbyte�n� a film by fungoval i bez nich (a jejich kvalita je neuv��iteln� r�znorod�). Se�teno podtr�eno, tohle je jin� komiks�rna, sprost�, odv�n� a procenty hodn� nadhodnocen�, ale i p�esto z�bavn�! PPS: je to vlastn� spr�vn�, aby si film uv�domoval s�m sebe a kritizoval okoln� universa nebo obsazen� rol�?:) To u� je na jinou diskuzi... PS: jsem s�m zv�dav, co je�t� se z prvn�ho Wolverina \"od��zne\" vzhledem k tomu, �e Deadpool m� sv�j restart, Gambita �ek� to sam�...(15.2.2016)\n" +
                "v�echny koment��e u�ivatele / [Trval� odkaz: http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10360720]\n" +
                "to je BOMBA ****\n" +
                "Po prvn�m shl�dnut� traileru jsem Deadpoolovi moc nev��il, ale nakonec mus�m ��ct, �e je to fakticky bomba. Nicm�n� zdej�� hodnocen� m� vysloven� vyt�hlo do kina, abych se nakonec s�m p�esv�d�il. V�sledek je takovej, �e Deadpool p�esn� splnil, pro co byl ur�enej. Prvn� chv�le sice byly dost rozpa�it�. Po p�l hodin� jsem nic moc nev�d�l, co si o filmu myslet, ale jakmile Deadpool rozjel trojboj v hl�kovan�, nem�l konkurenci a sypal to ze sebe jak b�bovi�ky p�sek. V tu chv�li jsem si u��val naprosto bo�� nar�ky na v�echny mo�n� a nemo�n� postavy superhrdinsk�ho univerza a p�em��lel nad t�m, jestli takovej film n�kdo za dvacet, t�icet let ocen�. Ve v�sledku je to stejn� ale jedno, proto�e tr�by se tvo�� te� a tady a ty v tuhle chv�li mluv� za v�e." +
                "<li> jm�no, p��jmen�, orion login, studentsk� ��slo.</li>" +
                "Tablet PC - Intel Atom Quad Core Z3735F, kapacitn� multidotykov� IPS 10.1\" LED 1280x800, Intel HD Graphics, RAM 2GB, 64GB eMMC, WiFi, Bluetooth 4.0, webkamera 2 Mpx + 5 Mpx, 2�l�nkov� baterie, Windows 10 Home 32bit + MS Office Mobile";

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

            var pText2 = _processing.ProcessWord("tr�by");
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
