using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace InformationRetrievalManager.NLP.Test
{
    /// <summary>
    /// Test <see cref="IndexProcessing"/>
    /// </summary>
    public class IndexProcessingTest : IDisposable
    {
        #region Private Members

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string _processingName = "utest_index";
        private IndexProcessing _processing;

        #endregion

        #region Constructor

        /// <summary>
        /// Do "global" initialization here; Called before every test method.
        /// </summary>
        public IndexProcessingTest(ITestOutputHelper testOutputHelper)
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

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, null, null, false, true, true);

            var text = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "�au�k");

            #endregion

            #region Act

            _processing.IndexDocument(text);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            Assert.True(vocabulary.ContainsKey("cau"));

            #endregion
        }

        /// <summary>
        /// Check HTML tags
        /// </summary>
        [Fact]
        public void Index_CheckHtml()
        {
            #region Arrange

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(), null, null, null, false, true, true);

            var text = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "<li>");

            #endregion

            #region Act

            _processing.IndexDocument(text);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            Assert.True(vocabulary.ContainsKey("<li>"));

            #endregion
        }

        /// <summary>
        /// Check links
        /// </summary>
        [Fact]
        public void Index_CheckLink()
        {
            #region Arrange

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(), null, null, null, false, true, true);

            var text = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10355101 link");

            #endregion

            #region Act

            _processing.IndexDocument(text);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            Assert.False(vocabulary.ContainsKey("http"));
            Assert.True(vocabulary.ContainsKey("http://www.csfd.cz/film/261379-deadpool/komentare/?comment=10355101"));

            #endregion
        }

        /// <summary>
        /// Check tokenization
        /// </summary>
        [Fact]
        public void Index_CheckTokenization()
        {
            #region Arrange

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(), null, null, null, false, false, false);

            var text1 = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "(p�st�).");
            var text2 = new IndexDocumentDataModel(1, string.Empty, string.Empty, default, "1280x800");
            var text3 = new IndexDocumentDataModel(2, string.Empty, string.Empty, default, "pr*sata");

            #endregion

            #region Act

            _processing.IndexDocument(text1);
            _processing.IndexDocument(text2);
            _processing.IndexDocument(text3);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            Assert.True(vocabulary.ContainsKey(_processing.ProcessWord("p�st�")));
            Assert.True(vocabulary.ContainsKey(_processing.ProcessWord("1280x800")));
            Assert.True(vocabulary.ContainsKey(_processing.ProcessWord("pr*sata")));

            #endregion
        }

        /// <summary>
        /// Check stop words
        /// </summary>
        [Fact]
        public void Index_CheckStopWords()
        {
            #region Arrange

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), new StopWordRemover(ProcessingLanguage.CZ), null, null, true, false, false);

            var text1 = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "T�mto textem britsk� The Guardian Jana �ul�ka nepot�il");
            var text2 = new IndexDocumentDataModel(1, string.Empty, string.Empty, default, "Jestli�e nep�ijde, m�l by se omluvit.");
            var text3 = new IndexDocumentDataModel(2, string.Empty, string.Empty, default, "�edes�t procent na�� elekt�iny vyr�b�me z dov�en�ho plynu, p�i�em� pr�m�r EU je okolo 40 %.");
            var text4 = new IndexDocumentDataModel(3, string.Empty, string.Empty, default, "Tento seznam m��e b�t revidov�n po dvou letech, p�i�em� samotn� programy mohou b�t p�edlo�eny jednou za rok.");
            var text5 = new IndexDocumentDataModel(4, string.Empty, string.Empty, default, "T�mto pozn�n�m p�ekro�uje �ensk� hnut� hranice feminismu a st�v� se hnut�m pokrokov�m, demokratick�m. Dozn�v�m, �e jsem byl zna�n� p�ekvapen t�mto nov�m d�kazem praktick� str�nky soudruhov�ch theori�. �echov� byli st�eni t�mt� osudem po stalet�ch hrdinsk�ho odboje proti n�meck�mu �tlaku. T�mto zp�sobem tak�, kdy� op�t chcete za��t pracovat, aktivn� spo�i� obrazovky ukon�ujete. T�m jste ur�ili, �e program vyhled� v�echny soubory, jejich� n�zev t�mto slovem za��n�. Patn�ct procent s t�mto n�zorem nesouhlasilo a p�tina mu�� nev�d�la, jak odpov�d�t. Celou tu poh�dku o t�ech Garridebech si patrn� vymyslel pr�v� za t�mto ��elem. Vlo��te znam�nko rovn� se ka�d� vzorec mus� t�mto znam�nkem za��nat. Zp�edu kr��el Stiv�n, za nim jel ryt�� mlad� a za t�mto klusal starec. P�ed t�mto gentlemanem m��ete ��ci v�e, co byste hodlal sv��it mn�. V�eobecn� dlu�no uznati, �e t�mto nejv�hodn��m m�stem je �kola. Cht�l bych V�s proto t�mto dopisem poprosit o odpov�� na tyto ot�zky. V�m to a r�da bych, abyste i vy za�al svou pr�ci s t�mto v�dom�m. Tehdej�� prezident Charles King mus� pod t�mto tlakem odstoupit. Poc�tila jsem ostr� bodnut� ��rlivosti nad t�mto cizincem. P�e, pohledem t�mto p�ekvapen, po cel�m t�le se t��sl. T�mto emailem bych se cht�la informovat o p��znac�ch AIDS. Hlava mi t�m�� vybuchla snahou uva�ovat t�mto zp�sobem. Nechala jsem t�mto temn�m zji�t�n�m naplnit sv� o�i. �tvrtina respondent� naopak s t�mto n�zorem souhlasila. Rance zd�l se b�ti pon�kud pozloben t�mto odbo�en�m. T�mto stra�liv�m zp�sobem jsem nabyl svoje d�dictv�. V��n� �koda, �e t�mto um�n�m pl�tvala na Dolpha. Nep�ipou�t�l jsem si t�mto sm�rem jedinou my�lenku. Dodavatel t�mto informuje odb�ratele v souladu s z�k. V�, �e mi nikdo neposlal kv�tiny t�mto zp�sobem. Ne, rozhodn� se nesnese s t�mto morbidn�m pan�kem. T�mto ve�erem se po�alo podiv�nstv� kapit�na J. Mohla b�t r�na zasazena t�mto p�edm�tem? Mohla. Jasnovidec byl z�ejm� polichocen t�mto uzn�n�m. ");
            var text6 = new IndexDocumentDataModel(5, string.Empty, string.Empty, default, "A�koli se cel� rok u�il, zn�mky na vysv�d�en� m� podpr�m�rn�.");

            #endregion

            #region Act

            _processing.IndexDocument(text1);
            _processing.IndexDocument(text2);
            _processing.IndexDocument(text3);
            _processing.IndexDocument(text4);
            _processing.IndexDocument(text5);
            _processing.IndexDocument(text6);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            Assert.True(vocabulary.ContainsKey(_processing.ProcessWord("t�mt�"))); // t�mto = stopword - expl. for IR ex.
            Assert.True(vocabulary.ContainsKey(_processing.ProcessWord("a�koli")));
            Assert.True(vocabulary.ContainsKey(_processing.ProcessWord("jestli�e")));
            Assert.True(vocabulary.ContainsKey(_processing.ProcessWord("p�i�em�")));

            #endregion
        }

        /// <summary>
        /// Check date
        /// </summary>
        [Fact]
        public void Index_CheckDate()
        {
            #region Arrange

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(), new StopWordRemover(), null, null, false, false, false);

            var text1 = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "11.2. 2015");
            var text2 = new IndexDocumentDataModel(1, string.Empty, string.Empty, default, "15.5.2010");

            #endregion

            #region Act

            _processing.IndexDocument(text1);
            _processing.IndexDocument(text2);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            Assert.True(vocabulary.ContainsKey("11.2."));
            Assert.True(vocabulary.ContainsKey("15.5.2010"));

            #endregion
        }

        /// <summary>
        /// Check diacritics
        /// </summary>
        [Fact]
        public void Index_CheckDiacritics()
        {
            #region Arrange

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, null, null, false, true, false);

            var text1 = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "�au");
            var text2 = new IndexDocumentDataModel(1, string.Empty, string.Empty, default, "cau");
            var text3 = new IndexDocumentDataModel(2, string.Empty, string.Empty, default, "ca�");
            var text4 = new IndexDocumentDataModel(3, string.Empty, string.Empty, default, "c�u");
            var text5 = new IndexDocumentDataModel(4, string.Empty, string.Empty, default, "�au");

            #endregion

            #region Act

            _processing.IndexDocument(text1);
            _processing.IndexDocument(text2);
            _processing.IndexDocument(text3);
            _processing.IndexDocument(text4);
            _processing.IndexDocument(text5);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            var pText = _processing.ProcessWord("�au");
            if (vocabulary.ContainsKey(pText))
                Assert.Equal(5, vocabulary[pText].Sum(o => o.Value.Frequency));
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

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, null, null, true, false, false);

            var text1 = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "BOMB");
            var text2 = new IndexDocumentDataModel(1, string.Empty, string.Empty, default, "Bomba");
            var text3 = new IndexDocumentDataModel(2, string.Empty, string.Empty, default, "bomba");
            var text4 = new IndexDocumentDataModel(3, string.Empty, string.Empty, default, "BOMBY");

            #endregion

            #region Act

            _processing.IndexDocument(text1);
            _processing.IndexDocument(text2);
            _processing.IndexDocument(text3);
            _processing.IndexDocument(text4);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            var pText = _processing.ProcessWord("bomb");
            if (vocabulary.ContainsKey(pText))
                Assert.Equal(4, vocabulary[pText].Sum(o => o.Value.Frequency));
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

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, null, null, false, false, false);

            var text1 = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "sm�j�");
            var text2 = new IndexDocumentDataModel(1, string.Empty, string.Empty, default, "sm�ju");
            var text3 = new IndexDocumentDataModel(2, string.Empty, string.Empty, default, "sm�je�");
            var text4 = new IndexDocumentDataModel(3, string.Empty, string.Empty, default, "sm�jeme");
            var text5 = new IndexDocumentDataModel(4, string.Empty, string.Empty, default, "sm�j�");
            var text6 = new IndexDocumentDataModel(5, string.Empty, string.Empty, default, "sm�jou");

            #endregion

            #region Act

            _processing.IndexDocument(text1);
            _processing.IndexDocument(text2);
            _processing.IndexDocument(text3);
            _processing.IndexDocument(text4);
            _processing.IndexDocument(text5);
            _processing.IndexDocument(text6);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            var pText1 = _processing.ProcessWord("sm�j�");
            if (vocabulary.ContainsKey(pText1))
                Assert.Equal(4, vocabulary[pText1].Sum(o => o.Value.Frequency));
            else
                Assert.True(false);

            var pText2 = _processing.ProcessWord("sm�je�");
            if (vocabulary.ContainsKey(pText2))
                Assert.Equal(1, vocabulary[pText2].Sum(o => o.Value.Frequency));
            else
                Assert.True(false);

            var pText3 = _processing.ProcessWord("sm�jeme");
            if (vocabulary.ContainsKey(pText3))
                Assert.Equal(1, vocabulary[pText3].Sum(o => o.Value.Frequency));
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

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, null, null, true, false, false);

            var text1 = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "BOMB");
            var text2 = new IndexDocumentDataModel(1, string.Empty, string.Empty, default, "Bomba");
            var text3 = new IndexDocumentDataModel(2, string.Empty, string.Empty, default, "bomba");
            var text4 = new IndexDocumentDataModel(3, string.Empty, string.Empty, default, "bomby");

            #endregion

            #region Act

            _processing.IndexDocument(text1);
            _processing.IndexDocument(text2);
            _processing.IndexDocument(text3);
            _processing.IndexDocument(text4);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            var pText = _processing.ProcessWord("bomb");
            if (vocabulary.ContainsKey(pText))
                Assert.Equal(4, vocabulary[pText].Sum(o => o.Value.Frequency));
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

            _processing = new IndexProcessing(_processingName, new Tokenizer(), new Stemmer(ProcessingLanguage.CZ), null, null, null, true, false, false);

            var text = new IndexDocumentDataModel(0, string.Empty, string.Empty, default, "��� jak se m�?" +
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
                "Tablet PC - Intel Atom Quad Core Z3735F, kapacitn� multidotykov� IPS 10.1\" LED 1280x800, Intel HD Graphics, RAM 2GB, 64GB eMMC, WiFi, Bluetooth 4.0, webkamera 2 Mpx + 5 Mpx, 2�l�nkov� baterie, Windows 10 Home 32bit + MS Office Mobile");

            #endregion

            #region Act

            _processing.IndexDocument(text);
            var vocabulary = _processing.InvertedIndex.GetReadOnlyVocabulary();
            PrintWordFrequencies(vocabulary);

            #endregion

            #region Assert

            var pText1 = _processing.ProcessWord("bomb");
            if (vocabulary.ContainsKey(pText1))
                Assert.Equal(2, vocabulary[pText1].Sum(o => o.Value.Frequency));
            else
                Assert.True(false);

            var pText2 = _processing.ProcessWord("tr�by");
            if (vocabulary.ContainsKey(pText2))
                Assert.Equal(2, vocabulary[pText2].Sum(o => o.Value.Frequency));
            else
                Assert.True(false);

            var pText3 = _processing.ProcessWord("z3735f");
            if (vocabulary.ContainsKey(pText3))
                Assert.Equal(1, vocabulary[pText3].Sum(o => o.Value.Frequency));
            else
                Assert.True(false);

            var pText4 = _processing.ProcessWord("</li>");
            if (vocabulary.ContainsKey(pText4))
                Assert.Equal(4, vocabulary[pText4].Sum(o => o.Value.Frequency));
            else
                Assert.True(false);

            #endregion
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Print word frequencies
        /// </summary>
        /// <param name="vocabulary">The vocabulary</param>
        private void PrintWordFrequencies(IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> vocabulary)
        {
            foreach (KeyValuePair<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> entry in vocabulary)
                _testOutputHelper.WriteLine(entry.Key + ":\t" + entry.Value.Sum(o => o.Value.Frequency));
            PrintSortedDictionary(vocabulary);
        }

        /// <summary>
        /// Print sorted words from the lsit of frequencies
        /// </summary>
        /// <param name="vocabulary">The vocabulary</param>
        private void PrintSortedDictionary(IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> vocabulary)
        {
            string[] words = vocabulary.Keys.ToArray();
            Array.Sort(words);
            _testOutputHelper.WriteLine(string.Join(", ", words));
        }

        #endregion
    }
}
