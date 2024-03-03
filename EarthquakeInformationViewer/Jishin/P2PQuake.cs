using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EarthquakeInformationViewer
{
    public class P2PQuake
    {
        public class DetailPrompt
        {
            public string Area { get; set; }
            public int AreaMaxIntn { get; set; }
        }

        public List<DetailPrompt> ConvertDetailToPrompt(P2PEqAPI DetailData)
        {
            List<DetailPrompt> respData = new List<DetailPrompt>();
            List<DetailPrompt> tempData = new List<DetailPrompt>();
            int maxint = 0;
            string area = "";
            DetailPrompt dp = new DetailPrompt();

            foreach(Point point in DetailData.points)
            {
                dp = new DetailPrompt() {
                    Area = ConvertCityToArea(point),
                    AreaMaxIntn = point.scale,
                };
                tempData.Add(dp);
            }

            foreach (DetailPrompt value in tempData)
            {
                if (tempData.Count == 1)
                {
                    dp = new DetailPrompt()
                    {
                        Area = value.Area,
                        AreaMaxIntn = value.AreaMaxIntn
                    };
                    respData.Add(dp);
                    return respData;
                }
                if(area == "" || area != value.Area)
                {
                    if(area != value.Area && area != "" && maxint != 0)
                    {
                        dp = new DetailPrompt()
                        {
                            Area = area,
                            AreaMaxIntn = maxint
                        };
                        respData.Add(dp);
                    }
                    area = value.Area;
                    maxint = value.AreaMaxIntn;
                }
                else if(area == value.Area)
                { 
                    if(value.AreaMaxIntn >= maxint)
                    {
                        maxint = value.AreaMaxIntn;
                    }
                }
            }
            return respData;
        }

        public string ConvertCityToArea(Point point)
        {
            string respData="";
            switch (point.pref)
            {
                case "北海道":
                    if (Regex.IsMatch(point.addr, "石狩市|当別町|新篠津村"))
                        respData = "石狩地方北部";
                    if (Regex.IsMatch(point.addr, "札幌市|江別市"))
                        respData = "石狩地方中部";
                    if (Regex.IsMatch(point.addr, "千歳市|恵庭市|北広島市"))
                        respData = "石狩地方南部";
                    if (Regex.IsMatch(point.addr, "八雲町|長万部町"))
                        respData = "渡島地方北部";
                    if (Regex.IsMatch(point.addr, "函館市|北斗市|七飯町|鹿部町|森町"))
                        respData = "渡島地方東部";
                    if (Regex.IsMatch(point.addr, "松前町|福島町|知内町|木古内町"))
                        respData = "渡島地方西部";
                    if (Regex.IsMatch(point.addr, "江差町|上ノ国町|厚沢部町|乙部町|今金町|せたな町"))
                        respData = "檜山地方";
                    if (Regex.IsMatch(point.addr, "小樽市|積丹町|古平町|仁木町|余市町|赤井川村"))
                        respData = "後志地方北部";
                    if (Regex.IsMatch(point.addr, "ニセコ町|真狩村|留寿都村|喜茂別町|京極町|倶知安町"))
                        respData = "後志地方東部";
                    if (Regex.IsMatch(point.addr, "島牧村|寿都町|黒松内町|蘭越町|共和町|岩内町|泊村|神恵内"))
                        respData = "後志地方西部";
                    if (Regex.IsMatch(point.addr, "奥尻町"))
                        respData = "北海道奥尻島";
                    if (Regex.IsMatch(point.addr, "深川市|妹背牛町|秩父別町|北竜町|沼田町"))
                        respData = "空知地方北部";
                    if (Regex.IsMatch(point.addr, "芦別市|赤平市|滝川市|砂川市|歌志内市|奈井江町|上砂川町|浦臼町|新十津川町|雨竜町"))
                        respData = "空知地方中部";
                    if (Regex.IsMatch(point.addr, "夕張市|岩見沢市|美唄市|三笠市|南幌町|由仁町|長沼町|栗山町|月形町"))
                        respData = "空知地方南部";
                    if (Regex.IsMatch(point.addr, "士別市|名寄市|和寒町|剣淵町|下川町|美深町|音威子府村|中川町|幌加内町"))
                        respData = "上川地方北部";
                    if (Regex.IsMatch(point.addr, "旭川市|鷹栖町|東神楽町|当麻町|比布町|愛別町|上川町|東川町|美瑛町"))
                        respData = "上川地方中部";
                    if (Regex.IsMatch(point.addr, "富良野市|上富良野町|中富良野町|南富良野町|占冠村"))
                        respData = "上川地方南部";
                    if (Regex.IsMatch(point.addr, "苫前町|羽幌町|初山別村|遠別町|天塩町"))
                        respData = "留萌地方中北部";
                    if (Regex.IsMatch(point.addr, "留萌市|増毛町|小平町"))
                        respData = "留萌地方南部";
                    if (Regex.IsMatch(point.addr, "稚内市|猿払村|豊富町|幌延町"))
                        respData = "宗谷地方北部";
                    if (Regex.IsMatch(point.addr, "浜頓別町|中頓別町|枝幸町"))
                        respData = "宗谷地方南部";
                    if (Regex.IsMatch(point.addr, "礼文町|利尻町|利尻富士町"))
                        respData = "北海道利尻礼文";
                    if (Regex.IsMatch(point.addr, "網走市|美幌町|津別町|大空町|斜里町|清里町|小清水町"))
                        respData = "網走地方";
                    if (Regex.IsMatch(point.addr, "北見市|訓子府町|置戸町|佐呂間町"))
                        respData = "北見地方";
                    if (Regex.IsMatch(point.addr, "紋別市|遠軽町|湧別町|滝上町|興部町|西興部村|雄武町"))
                        respData = "紋別地方";
                    if (Regex.IsMatch(point.addr, "伊達市|豊浦町|洞爺湖町|壮瞥町"))
                        respData = "胆振地方西部";
                    if (Regex.IsMatch(point.addr, "室蘭市|苫小牧市|登別市|白老町|安平町|厚真町|むかわ町"))
                        respData = "胆振地方中東部";
                    if (Regex.IsMatch(point.addr, "日高町|平取町"))
                        respData = "日高地方西部";
                    if (Regex.IsMatch(point.addr, "新冠町|新ひだか町"))
                        respData = "日高地方中部";
                    if (Regex.IsMatch(point.addr, "浦河町|様似町|えりも町"))
                        respData = "日高地方東部";
                    if (Regex.IsMatch(point.addr, "上士幌町|鹿追町|新得町|足寄町|陸別町"))
                        respData = "十勝地方北部";
                    if (Regex.IsMatch(point.addr, "帯広市|音更町|士幌町|清水町|芽室町|幕別町|池田町|豊頃町|本別町|浦幌町"))
                        respData = "十勝地方中部";
                    if (Regex.IsMatch(point.addr, "中札内村|更別村|大樹町|広尾町"))
                        respData = "十勝地方南部";
                    if (Regex.IsMatch(point.addr, "弟子屈町"))
                        respData = "釧路地方北部";
                    if (Regex.IsMatch(point.addr, "釧路市|釧路町|厚岸町|浜中町|標茶町|鶴居村|白糠町"))
                        respData = "釧路地方中南部";
                    if (Regex.IsMatch(point.addr, "中標津町|標津町|羅臼町"))
                        respData = "根室地方北部";
                    if (Regex.IsMatch(point.addr, "別海町"))
                        respData = "根室地方中部";
                    if (Regex.IsMatch(point.addr, "根室市"))
                        respData = "根室地方南部";
                    break;
                case "青森県":
                    if (Regex.IsMatch(point.addr, "青森市|五所川原市|つがる市|平内町|今別町|蓬田村|外ヶ浜町|板柳町|鶴田町|中泊町"))
                        respData = "青森県津軽北部";
                    if (Regex.IsMatch(point.addr, "弘前市|黒石市|平川市|鰺ヶ沢町|深浦町|西目屋村|藤崎町|大鰐町|田舎館村"))
                        respData = "青森県津軽南部";
                    if (Regex.IsMatch(point.addr, "八戸市|十和田市|三沢市|上北郡［野辺地町|七戸町|六戸町|横浜町|東北町|六ヶ所村|おいらせ町|三戸町|五戸町|田子町|南部町|階上町|新郷村"))
                        respData = "青森県三八上北";
                    if (Regex.IsMatch(point.addr, "むつ市|大間町|東通村|風間浦村|佐井村"))
                        respData = "青森県下北";
                    break;
                case "岩手県":
                    if (Regex.IsMatch(point.addr, "宮古市|久慈市|山田町|岩泉町|田野畑村|普代村|洋野町|野田村"))
                        respData = "岩手県沿岸北部";
                    if (Regex.IsMatch(point.addr, "大船渡市|陸前高田市|釜石市|住田町|大槌町"))
                        respData = "岩手県沿岸南部";
                    if (Regex.IsMatch(point.addr, "盛岡市|二戸市|八幡平市|滝沢市|雫石町|葛巻町|岩手町|紫波町|矢巾町|軽米町|九戸村|一戸町"))
                        respData = "岩手県内陸北部";
                    if (Regex.IsMatch(point.addr, "花巻市|北上市|遠野市|一関市|奥州市|西和賀町|金ケ崎町|平泉町"))
                        respData = "岩手県内陸南部";
                    break;
                case "宮城県":
                    if (Regex.IsMatch(point.addr, "気仙沼市|登米市|栗原市|大崎市|色麻町|加美町|涌谷町|美里町|南三陸町"))
                        respData = "宮城県北部";
                    if (Regex.IsMatch(point.addr, "仙台市|石巻市|塩竈市|多賀城市|東松島市|富谷市|松島町|七ヶ浜町|利府町|大和町|大郷町|大衡村|女川町"))
                        respData = "宮城県中部";
                    if (Regex.IsMatch(point.addr, "白石市|名取市|角田市|岩沼市|蔵王町|七ヶ宿町|大河原町|村田町|柴田町|川崎町|丸森町|亘理町|山元町"))
                        respData = "宮城県南部";
                    break;
                case "秋田県":
                    if (Regex.IsMatch(point.addr, "能代市|男鹿市|潟上市|藤里町|三種町|八峰町|五城目町|八郎潟町|井川町|大潟村"))
                        respData = "秋田県沿岸北部";
                    if (Regex.IsMatch(point.addr, "秋田市|由利本荘市|にかほ市"))
                        respData = "秋田県沿岸南部";
                    if (Regex.IsMatch(point.addr, "大館市|鹿角市|北秋田市|小坂町|上小阿仁村"))
                        respData = "秋田県内陸北部";
                    if (Regex.IsMatch(point.addr, "横手市|湯沢市|大仙市|仙北市|美郷町|羽後町|東成瀬村"))
                        respData = "秋田県内陸南部";
                    break;
                case "山形県":
                    if (Regex.IsMatch(point.addr, "鶴岡市|酒田市|三川町|庄内町|遊佐町"))
                        respData = "山形県庄内";
                    if (Regex.IsMatch(point.addr, "新庄市|金山町|最上町|舟形町|真室川町|大蔵村|鮭川村|戸沢村"))
                        respData = "山形県最上";
                    if (Regex.IsMatch(point.addr, "山形市|寒河江市|上山市|村山市|天童市|東根市|尾花沢市|山辺町|中山町|河北町|西川町|朝日町|大江町|大石田町"))
                        respData = "山形県村山";
                    if (Regex.IsMatch(point.addr, "米沢市|長井市|南陽市|高畠町|川西町|小国町|白鷹町|飯豊町"))
                        respData = "山形県置賜";
                    break;
                case "福島県":
                    if (Regex.IsMatch(point.addr, "福島市|郡山市|白河市|須賀川市|二本松市|田村市|伊達市|本宮市|伊達郡［桑折町|国見町|川俣町|大玉村|鏡石町|天栄村|西郷村|泉崎村|中島村|矢吹町|棚倉町|矢祭町|塙町|鮫川村|石川町|玉川村|平田村|浅川町|古殿町|三春町|小野町"))
                        respData = "福島県中通り";
                    if (Regex.IsMatch(point.addr, "いわき市|相馬市|南相馬市|広野町|楢葉町|富岡町|川内村|大熊町|双葉町|浪江町|葛尾村］|新地町|飯舘村"))
                        respData = "福島県浜通り";
                    if (Regex.IsMatch(point.addr, "会津若松市|喜多方市|下郷町|檜枝岐村|只見町|南会津町|北塩原村|西会津町|磐梯町|猪苗代町|会津坂下町|湯川村|柳津町|三島町|金山町|昭和村|会津美里町"))
                        respData = "福島県会津";
                    break;
                case "茨城県":
                    if (Regex.IsMatch(point.addr, "水戸市|日立市|常陸太田市|高萩市|北茨城市|笠間市|ひたちなか市|常陸大宮市|那珂市|小美玉市|茨城町|大洗町|城里町|東海村|大子町"))
                        respData = "茨城県北部";
                    if (Regex.IsMatch(point.addr, "土浦市|古河市|石岡市|結城市|龍ケ崎市|下妻市|常総市|取手市|牛久市|つくば市|鹿嶋市|潮来市|守谷市|筑西市|坂東市|稲敷市|かすみがうら市|桜川市|神栖市|行方市|鉾田市|つくばみらい市|稲敷郡［美浦村|阿見町|河内町|八千代町|五霞町|境町|利根町"))
                        respData = "茨城県南部";
                    break;
                case "栃木県":
                    if (Regex.IsMatch(point.addr, "日光市|大田原市|矢板市|那須塩原市|塩谷町|那須町"))
                        respData = "栃木県北部";
                    if (Regex.IsMatch(point.addr, "宇都宮市|足利市|栃木市|佐野市|鹿沼市|小山市|真岡市|さくら市|那須烏山市|下野市|上三川町|益子町|茂木町|市貝町|芳賀町|壬生町|野木町|高根沢町|那珂川町"))
                        respData = "栃木県南部";
                    break;
                case "群馬県":
                    if (Regex.IsMatch(point.addr, "沼田市|中之条町|長野原町|嬬恋村|草津町|高山村|東吾妻町|片品村|川場村|昭和村|みなかみ町"))
                        respData = "群馬県北部";
                    if (Regex.IsMatch(point.addr, "前橋市|高崎市|桐生市|伊勢崎市|太田市|館林市|渋川市|藤岡市|富岡市|安中市|みどり市|榛東村|吉岡町|上野村|神流町|下仁田町|南牧村|甘楽町|玉村町|板倉町|明和町|千代田町|大泉町|邑楽町"))
                        respData = "群馬県南部";
                    break;
                case "埼玉県":
                    if (Regex.IsMatch(point.addr, "熊谷市|行田市|加須市|本庄市|東松山市|羽生市|鴻巣市|深谷市|久喜市|滑川町|嵐山町|小川町|ときがわ町|吉見町|鳩山町|東秩父村|美里町|神川町|上里町|寄居町"))
                        respData = "埼玉県北部";
                    if (Regex.IsMatch(point.addr, "さいたま市|川越市|川口市|所沢市|飯能市|春日部市|狭山市|上尾市|草加市|越谷市|蕨市|戸田市|入間市|朝霞市|志木市|和光市|新座市|桶川市|北本市|八潮市|富士見市|三郷市|蓮田市|坂戸市|幸手市|鶴ヶ島市|日高市|吉川市|ふじみ野市|白岡市|伊奈町|三芳町|毛呂山町|越生町|川島町|宮代町|杉戸町|松伏町"))
                        respData = "埼玉県南部";
                    if (Regex.IsMatch(point.addr, "秩父市|横瀬町|皆野町|長瀞町|小鹿野町"))
                        respData = "埼玉県秩父";
                    break;
                case "千葉県":
                    if (Regex.IsMatch(point.addr, "銚子市|茂原市|東金市|旭市|匝瑳市|香取市|山武市|大網白里市|神崎町|多古町|東庄町|九十九里町|芝山町|横芝光町|一宮町|睦沢町|長生村|白子町|長柄町|長南町"))
                        respData = "千葉県北東部";
                    if (Regex.IsMatch(point.addr, "千葉市|市川市|船橋市|松戸市|野田市|成田市|佐倉市|習志野市|柏市|市原市|流山市|八千代市|我孫子市|鎌ケ谷市|浦安市|四街道市|八街市|印西市|白井市|富里市|酒々井町|栄町"))
                        respData = "千葉県北西部";
                    if (Regex.IsMatch(point.addr, "館山市|木更津市|勝浦市|鴨川市|君津市|富津市|袖ケ浦市|南房総市|いすみ市|大多喜町|御宿町|鋸南町"))
                        respData = "千葉県南部";
                    break;
                case "東京都":
                    if (Regex.IsMatch(point.addr, "千代田区|中央区|港区|新宿区|文京区|台東区|墨田区|江東区|品川区|目黒区|大田区|世田谷区|渋谷区|中野区|杉並区|豊島区|北区|荒川区|板橋区|練馬区|足立区|葛飾区|江戸川区"))
                        respData = "東京都２３区";
                    if (Regex.IsMatch(point.addr, "八王子市|立川市|武蔵野市|三鷹市|府中市|昭島市|調布市|町田市|小金井市|小平市|日野市|東村山市|国分寺市|国立市|福生市|狛江市|東大和市|清瀬市|東久留米市|武蔵村山市|多摩市|稲城市|羽村市|西東京市|瑞穂町"))
                        respData = "東京都多摩東部";
                    if (Regex.IsMatch(point.addr, "青梅市|あきる野市|日の出町|檜原村|奥多摩町"))
                        respData = "東京都多摩西部";
                    if (Regex.IsMatch(point.addr, "大島町"))
                        respData = "伊豆大島";
                    if (Regex.IsMatch(point.addr, "利島村|新島村"))
                        respData = "新島";
                    if (Regex.IsMatch(point.addr, "神津島村"))
                        respData = "神津島";
                    if (Regex.IsMatch(point.addr, "三宅村|御蔵島村"))
                        respData = "三宅島";
                    if (Regex.IsMatch(point.addr, "八丈町|青ヶ島村"))
                        respData = "八丈島";
                    if (Regex.IsMatch(point.addr, "小笠原村"))
                        respData = "小笠原";
                    break;
                case "神奈川県":
                    if (Regex.IsMatch(point.addr, "横浜市|川崎市|横須賀市|平塚市|鎌倉市|藤沢市|茅ヶ崎市|逗子市|三浦市|大和市|海老名市|座間市|綾瀬市|葉山町|寒川町|大磯町|二宮町"))
                        respData = "神奈川県東部";
                    if (Regex.IsMatch(point.addr, "小田原市|相模原市|秦野市|厚木市|伊勢原市|南足柄市|中井町|大井町|松田町|山北町|開成町|箱根町|真鶴町|湯河原町|愛川町|清川村"))
                        respData = "神奈川県西部";
                    break;
                case "新潟県":
                    if (Regex.IsMatch(point.addr, "糸魚川市|妙高市|上越市"))
                        respData = "新潟県上越";
                    if (Regex.IsMatch(point.addr, "長岡市|三条市|柏崎市|小千谷市|加茂市|十日町市|見附市|魚沼市|南魚沼市|田上町|出雲崎町|湯沢町|津南町|刈羽村"))
                        respData = "新潟県中越";
                    if (Regex.IsMatch(point.addr, "新潟市|新発田市|村上市|燕市|五泉市|阿賀野市|胎内市|聖籠町|弥彦村|阿賀町|関川村|粟島浦村"))
                        respData = "新潟県下越";
                    if (Regex.IsMatch(point.addr, "佐渡市"))
                        respData = "新潟県佐渡";
                    break;
                case "富山県":
                    if (Regex.IsMatch(point.addr, "富山市|魚津市|滑川市|黒部市|舟橋村|上市町|立山町|入善町|朝日町"))
                        respData = "富山県東部";
                    if (Regex.IsMatch(point.addr, "高岡市|氷見市|砺波市|小矢部市|南砺市|射水市"))
                        respData = "富山県西部";
                    break;
                case "石川県":
                    if (Regex.IsMatch(point.addr, "七尾市|輪島市|珠洲市|羽咋市|志賀町|宝達志水町|中能登町|穴水町|能登町"))
                        respData = "石川県能登";
                    if (Regex.IsMatch(point.addr, "金沢市|小松市|加賀市|かほく市|白山市|能美市|野々市市|川北町|津幡町|内灘町"))
                        respData = "石川県加賀";
                    break;
                case "福井県":
                    if (Regex.IsMatch(point.addr, "福井市|大野市|勝山市|鯖江市|あわら市|越前市|坂井市|永平寺町|池田町|南越前町|越前町"))
                        respData = "福井県嶺北";
                    if (Regex.IsMatch(point.addr, "敦賀市|小浜市|美浜町|高浜町|おおい町|若狭町"))
                        respData = "福井県嶺南";
                    break;
                case "山梨県":
                    if (Regex.IsMatch(point.addr, "富士吉田市|都留市|大月市|上野原市|道志村|西桂町|忍野村|山中湖村|鳴沢村|富士河口湖町|小菅村|丹波山村"))
                        respData = "山梨県東部・富士五湖";
                    if (Regex.IsMatch(point.addr, "甲府市|山梨市|韮崎市|南アルプス市|北杜市|甲斐市|笛吹市|甲州市|中央市|市川三郷町|早川町|身延町|南部町|富士川町|昭和町"))
                        respData = "山梨県中・西部";
                    break;
                case "長野県":
                    if (Regex.IsMatch(point.addr, "長野市|須坂市|中野市|大町市|飯山市|千曲市|池田町|松川村|白馬村|小谷村|坂城町|小布施町|高山村|山ノ内町|木島平村|野沢温泉村|信濃町|小川村|飯綱町|栄村"))
                        respData = "長野県北部";
                    if (Regex.IsMatch(point.addr, "松本市|上田市|岡谷市|諏訪市|小諸市|茅野市|塩尻市|佐久市|東御市|安曇野市|小海町|川上村|南牧村|南相木村|北相木村|佐久穂町|軽井沢町|御代田町|立科町|青木村|長和町|下諏訪町|富士見町|原村|麻績村|生坂村|山形村|朝日村|筑北村"))
                        respData = "長野県中部";
                    if (Regex.IsMatch(point.addr, "飯田市|伊那市|駒ヶ根市|辰野町|箕輪町|飯島町|南箕輪村|中川村|宮田村|松川町|高森町|阿南町|阿智村|平谷村|根羽村|下條村|売木村|天龍村|泰阜村|喬木村|豊丘村|大鹿村|上松町|南木曽町|木祖村|王滝村|大桑村|木曽町"))
                        respData = "長野県南部";
                    break;
                case "岐阜県":
                    if (Regex.IsMatch(point.addr, "高山市|飛騨市|下呂市|白川村"))
                        respData = "岐阜県飛騨";
                    if (Regex.IsMatch(point.addr, "多治見市|中津川市|瑞浪市|恵那市|美濃加茂市|土岐市|可児市|坂祝町|富加町|川辺町|七宗町|八百津町|白川町|東白川村|御嵩町"))
                        respData = "岐阜県美濃東部";
                    if (Regex.IsMatch(point.addr, "岐阜市|大垣市|関市|美濃市|羽島市|各務原市|山県市|瑞穂市|本巣市|郡上市|海津市|岐南町|笠松町|養老町|垂井町|関ケ原町|神戸町|輪之内町|安八町|揖斐川町|大野町|池田町|北方町"))
                        respData = "岐阜県美濃中西部";
                    break;
                case "静岡県":
                    if (Regex.IsMatch(point.addr, "熱海市|伊東市|下田市|伊豆市|伊豆の国市|東伊豆町|河津町|南伊豆町|松崎町|西伊豆町|函南町"))
                        respData = "静岡県伊豆";
                    if (Regex.IsMatch(point.addr, "沼津市|三島市|富士宮市|富士市|御殿場市|裾野市|清水町|長泉町|小山町"))
                        respData = "静岡県東部";
                    if (Regex.IsMatch(point.addr, "静岡市|島田市|焼津市|藤枝市|牧之原市|吉田町|川根本町"))
                        respData = "静岡県中部";
                    if (Regex.IsMatch(point.addr, "浜松市|磐田市|掛川市|袋井市|湖西市|御前崎市|菊川市|森町"))
                        respData = "静岡県西部";
                    break;
                case "愛知県":
                    if (Regex.IsMatch(point.addr, "豊橋市|豊川市|蒲郡市|新城市|田原市|設楽町|東栄町|豊根村"))
                        respData = "愛知県東部";
                    if (Regex.IsMatch(point.addr, "名古屋市|岡崎市|一宮市|瀬戸市|半田市|春日井市|津島市|碧南市|刈谷市|豊田市|安城市|西尾市|犬山市|常滑市|江南市|小牧市|稲沢市|東海市|大府市|知多市|知立市|尾張旭市|高浜市|岩倉市|豊明市|日進市|愛西市|清須市|北名古屋市|弥富市|みよし市|あま市|長久手市|東郷町|西豊山町|大口町|扶桑町|大治町|蟹江町|飛島村|阿久比町|東浦町|南知多町|美浜町|武豊町|幸田町"))
                        respData = "愛知県西部";
                    break;
                case "三重県":
                    if (Regex.IsMatch(point.addr, "四日市市|桑名市|鈴鹿市|亀山市|いなべ市|木曽岬町|東員町|菰野町|朝日町|川越町"))
                        respData = "三重県北部";
                    if (Regex.IsMatch(point.addr, "津市|松阪市|名張市|伊賀市|多気町|明和町"))
                        respData = "三重県中部";
                    if (Regex.IsMatch(point.addr, "伊勢市|尾鷲市|鳥羽市|熊野市|志摩市|大台町|玉城町|度会町|大紀町|南伊勢町|紀北町|御浜町|紀宝町"))
                        respData = "三重県南部";
                    break;
                case "滋賀県":
                    if (Regex.IsMatch(point.addr, "彦根市|長浜市|高島市|米原市|愛荘町|豊郷町|甲良町|多賀町"))
                        respData = "滋賀県北部";
                    if (Regex.IsMatch(point.addr, "大津市|近江八幡市|草津市|守山市|栗東市|甲賀市|野洲市|湖南市|東近江市|日野町|竜王町"))
                        respData = "滋賀県南部";
                    break;
                case "京都府":
                    if (Regex.IsMatch(point.addr, "福知山市|舞鶴市|綾部市|宮津市|京丹後市|伊根町|与謝野町"))
                        respData = "京都府北部";
                    if (Regex.IsMatch(point.addr, "京都北区|京都上京区|京都左京区|京都中京区|京都東山区|京都下京区|京都南区|京都右京区|京都伏見区|京都山科区|京都西京区|宇治市|亀岡市|城陽市|向日市|長岡京市|八幡市|京田辺市|南丹市|木津川市|大山崎町|久御山町|井手町|宇治田原町|笠置町|和束町|精華町|南山城村|京丹波町"))
                        respData = "京都府南部";
                    break;
                case "大阪府":
                    if (Regex.IsMatch(point.addr, "大阪都島区|大阪福島区|大阪此花区|大阪西区|大阪港区|大阪大正区|大阪天王寺区|大阪浪速区|大阪西淀川区|大阪東淀川区|大阪東成区|大阪生野区|大阪旭区|大阪城東区|大阪阿倍野区|大阪住吉区|大阪東住吉区|大阪西成区|大阪淀川区|大阪鶴見区|大阪住之江区|大阪平野区|大阪北区|大阪中央区|豊中市|池田市|吹田市|高槻市|守口市|枚方市|茨木市|八尾市|寝屋川市|大東市|箕面市|柏原市|門真市|摂津市|東大阪市|四條畷市|交野市|島本町|豊能町|能勢町"))
                        respData = "大阪府北部";
                    if (Regex.IsMatch(point.addr, "大阪堺市堺区|大阪堺市中区|大阪堺市東区|大阪堺市西区|大阪堺市南区|大阪堺市北区|大阪堺市美原区|岸和田市|泉大津市|貝塚市|泉佐野市|富田林市|河内長野市|松原市|大阪和泉市|羽曳野市|高石市|藤井寺市|泉南市|大阪狭山市|阪南市|忠岡町|熊取町|田尻町|大阪岬町|大阪太子町|河南町|千早赤阪村"))
                        respData = "大阪府南部";
                    break;
                case "兵庫県":
                    if (Regex.IsMatch(point.addr, "豊岡市|養父市|朝来市|兵庫香美町|新温泉町"))
                        respData = "兵庫県北部";
                    if (Regex.IsMatch(point.addr, "神戸東灘区|神戸灘区|神戸兵庫区|神戸長田区|神戸須磨区|神戸垂水区|神戸北区|神戸中央区|神戸西区|尼崎市|明石市|西宮市|芦屋市|伊丹市|加古川市|西脇市|宝塚市|三木市|高砂市|川西市|小野市|三田市|加西市|丹波篠山市|丹波市|加東市|猪名川町|多可町|兵庫稲美町|播磨町"))
                        respData = "兵庫県南東部";
                    if (Regex.IsMatch(point.addr, "姫路市|相生市|赤穂市|宍粟市|たつの市|市川町|福崎町|兵庫神河町|兵庫太子町|上郡町|佐用町"))
                        respData = "兵庫県南西部";
                    if (Regex.IsMatch(point.addr, "洲本市|南あわじ市|淡路市"))
                        respData = "兵庫県淡路島";
                    break;
                case "奈良県":
                    if (Regex.IsMatch(point.addr, "奈良市|大和高田市|大和郡山市|天理市|橿原市|桜井市|五條市|御所市|生駒市|香芝市|葛城市|宇陀市|山添村|平群町|三郷町|斑鳩町|安堵町|奈良川西町|三宅町|田原本町|曽爾村|御杖村|高取町|明日香村|上牧町|王寺町|広陵町|河合町|吉野町|大淀町|下市町|黒滝村|天川村|野迫川村|十津川村|下北山村|上北山村|奈良川上村|東吉野村"))
                        respData = "奈良県";
                    break;
                case "和歌山県":
                    if (Regex.IsMatch(point.addr, "和歌山市|海南市|橋本市|有田市|御坊市|紀の川市|岩出市|紀美野町|かつらぎ町|九度山町|高野町|湯浅町|和歌山広川町|有田川町|和歌山美浜町|和歌山日高町|由良町|和歌山印南町|みなべ町|日高川町"))
                        respData = "和歌山県北部";
                    if (Regex.IsMatch(point.addr, "田辺市|新宮市|白浜町|上富田町|すさみ町|那智勝浦町|太地町|古座川町|北山村|串本町"))
                        respData = "和歌山県南部";
                    break;
                case "鳥取県":
                    if (Regex.IsMatch(point.addr, "鳥取市|岩美町|鳥取若桜町|智頭町|八頭町"))
                        respData = "鳥取県東部";
                    if (Regex.IsMatch(point.addr, "倉吉市|三朝町|湯梨浜町|琴浦町|北栄町"))
                        respData = "鳥取県中部";
                    if (Regex.IsMatch(point.addr, "米子市|境港市|日吉津村|大山町|鳥取南部町|伯耆町|日南町|鳥取日野町|江府町"))
                        respData = "鳥取県西部";
                    break;
                case "島根県":
                    if (Regex.IsMatch(point.addr, "松江市|出雲市|安来市|雲南市|奥出雲町|飯南町"))
                        respData = "島根県東部";
                    if (Regex.IsMatch(point.addr, "浜田市|益田市|大田市|江津市|川本町|島根美郷町|邑南町|津和野町|吉賀町"))
                        respData = "島根県西部";
                    if (Regex.IsMatch(point.addr, "海士町|西ノ島町|知夫村|隠岐の島町"))
                        respData = "島根県隠岐";
                    break;
                case "岡山県":
                    if (Regex.IsMatch(point.addr, "津山市|新見市|真庭市|美作市|新庄村|鏡野町|勝央町|奈義町|西粟倉村|久米南町|岡山美咲町"))
                        respData = "岡山県北部";
                    if (Regex.IsMatch(point.addr, "岡山北区|岡山中区|岡山東区|岡山南区|倉敷市|玉野市|笠岡市|井原市|総社市|高梁市|備前市|瀬戸内市|赤磐市|浅口市|和気町|早島町|里庄町|矢掛町|吉備中央町"))
                        respData = "岡山県南部";
                    break;
                case "広島県":
                    if (Regex.IsMatch(point.addr, "広島三次市|庄原市|安芸高田市|安芸太田町|北広島町"))
                        respData = "広島県北部";
                    if (Regex.IsMatch(point.addr, "三原市|尾道市|福山市|広島府中市|世羅町|神石高原町"))
                        respData = "広島県南東部";
                    if (Regex.IsMatch(point.addr, "広島中区|広島東区|広島南区|広島西区|広島安佐南区|広島安佐北区|広島安芸区|広島佐伯区|呉市|竹原市|大竹市|東広島市|廿日市市|江田島市|府中町|海田町|熊野町|坂町|大崎上島町"))
                        respData = "広島県南西部";
                    break;
                case "徳島県":
                    if (Regex.IsMatch(point.addr, "徳島市|鳴門市|小松島市|吉野川市|阿波市|美馬市|徳島三好市|佐那河内村|石井町|神山町|松茂町|北島町|藍住町|板野町|上板町|つるぎ町|東みよし町"))
                        respData = "徳島県北部";
                    if (Regex.IsMatch(point.addr, "阿南市|勝浦町|上勝町|那賀町|牟岐町|美波町|海陽町"))
                        respData = "徳島県南部";
                    break;
                case "香川県":
                    if (Regex.IsMatch(point.addr, "高松市|さぬき市|東かがわ市|土庄町|小豆島町|三木町|直島町"))
                        respData = "香川県東部";
                    if (Regex.IsMatch(point.addr, "丸亀市|坂出市|善通寺市|観音寺市|三豊市|宇多津町|綾川町|琴平町|多度津町|まんのう町"))
                        respData = "香川県西部";
                    break;
                case "愛媛県":
                    if (Regex.IsMatch(point.addr, "今治市|新居浜市|西条市|四国中央市|上島町"))
                        respData = "愛媛県東予";
                    if (Regex.IsMatch(point.addr, "松山市|伊予市|東温市|久万高原町|愛媛松前町|砥部町"))
                        respData = "愛媛県中予";
                    if (Regex.IsMatch(point.addr, "宇和島市|八幡浜市|大洲市|西予市|内子町|伊方町|松野町|愛媛鬼北町|愛南町"))
                        respData = "愛媛県南予";
                    break;
                case "高知県":
                    if (Regex.IsMatch(point.addr, "室戸市|安芸市|東洋町|奈半利町|田野町|安田町|北川村|馬路村|芸西村"))
                        respData = "高知県東部";
                    if (Regex.IsMatch(point.addr, "高知市|南国市|土佐市|須崎市|香南市|香美市|本山町|大豊町|土佐町|大川村|いの町|仁淀川町|佐川町|越知町|日高村"))
                        respData = "高知県中部";
                    if (Regex.IsMatch(point.addr, "宿毛市|土佐清水市|四万十市|中土佐町|梼原町|高知津野町|四万十町|大月町|三原村|黒潮町"))
                        respData = "高知県西部";
                    break;
                case "山口県":
                    if (Regex.IsMatch(point.addr, "萩市|長門市|美祢市|阿武町"))
                        respData = "山口県北部";
                    if (Regex.IsMatch(point.addr, "下関市|宇部市|山陽小野田市"))
                        respData = "山口県西部";
                    if (Regex.IsMatch(point.addr, "岩国市|光市|柳井市|周防大島町|和木町|上関町|田布施町|平生町"))
                        respData = "山口県東部";
                    if (Regex.IsMatch(point.addr, "山口市|防府市|下松市|周南市"))
                        respData = "山口県中部";
                    break;
                case "福岡県":
                    if (Regex.IsMatch(point.addr, "福岡東区|福岡博多区|福岡中央区|福岡南区|福岡西区|福岡城南区|福岡早良区|筑紫野市|春日市|大野城市|宗像市|太宰府市|福岡古賀市|福津市|糸島市|那珂川市|宇美町|篠栗町|志免町|須恵町|新宮町|久山町|粕屋町"))
                        respData = "福岡県福岡";
                    if (Regex.IsMatch(point.addr, "北九州門司区|北九州若松区|北九州戸畑区|北九州小倉北区|北九州小倉南区|北九州八幡東区|北九州八幡西区|行橋市|豊前市|中間市|芦屋町|水巻町|岡垣町|遠賀町|苅田町|みやこ町|吉富町|上毛町|築上町"))
                        respData = "福岡県北九州";
                    if (Regex.IsMatch(point.addr, "直方市|飯塚市|田川市|宮若市|嘉麻市|小竹町|鞍手町|桂川町|香春町|添田町|糸田町|福岡川崎町|大任町|赤村|福智町"))
                        respData = "福岡県筑豊";
                    if (Regex.IsMatch(point.addr, "大牟田市|久留米市|柳川市|八女市|筑後市|大川市|小郡市|うきは市|朝倉市|みやま市|筑前町|東峰村|大刀洗町|大木町|福岡広川町"))
                        respData = "福岡県筑後";
                    break;
                case "佐賀県":
                    if (Regex.IsMatch(point.addr, "唐津市|伊万里市|玄海町|有田町"))
                        respData = "佐賀県北部";
                    if (Regex.IsMatch(point.addr, "佐賀市|鳥栖市|多久市|武雄市|佐賀鹿島市|小城市|嬉野市|神埼市|吉野ヶ里町|基山町|上峰町|みやき町|大町町|江北町|白石町|太良町"))
                        respData = "佐賀県南部";
                    break;
                case "長崎県":
                    if (Regex.IsMatch(point.addr, "佐世保市|平戸市|松浦市|東彼杵町|川棚町|波佐見町|佐々町"))
                        respData = "長崎県北部";
                    if (Regex.IsMatch(point.addr, "長崎市|諫早市|大村市|西海市|長与町|時津町"))
                        respData = "長崎県南西部";
                    if (Regex.IsMatch(point.addr, "島原市|雲仙市|南島原市"))
                        respData = "長崎県島原半島";
                    if (Regex.IsMatch(point.addr, "対馬市"))
                        respData = "長崎県対馬";
                    if (Regex.IsMatch(point.addr, "壱岐市"))
                        respData = "長崎県壱岐";
                    if (Regex.IsMatch(point.addr, "佐世保市宇久島|五島市|小値賀町|新上五島町"))
                        respData = "長崎県五島";
                    break;
                case "熊本県":
                    if (Regex.IsMatch(point.addr, "阿蘇市|南小国町|熊本小国町|産山村|熊本高森町|南阿蘇村"))
                        respData = "熊本県阿蘇";
                    if (Regex.IsMatch(point.addr, "熊本中央区|熊本東区|熊本西区|熊本南区|熊本北区|八代市|荒尾市|玉名市|山鹿市|菊池市|宇土市|宇城市|合志市|熊本美里町|玉東町|南関町|長洲町|和水町|大津町|菊陽町|西原村|御船町|嘉島町|益城町|甲佐町|山都町|氷川町"))
                        respData = "熊本県熊本";
                    if (Regex.IsMatch(point.addr, "人吉市|錦町|多良木町|湯前町|水上村|相良村|五木村|山江村|球磨村|あさぎり町"))
                        respData = "熊本県球磨";
                    if (Regex.IsMatch(point.addr, "水俣市|上天草市|天草市|芦北町|津奈木町|苓北町"))
                        respData = "熊本県天草・芦北";
                    break;
                case "大分県":
                    if (Regex.IsMatch(point.addr, "中津市|豊後高田市|宇佐市|国東市|姫島村"))
                        respData = "大分県北部";
                    if (Regex.IsMatch(point.addr, "大分市|別府市|臼杵市|津久見市|杵築市|由布市|日出町"))
                        respData = "大分県中部";
                    if (Regex.IsMatch(point.addr, "佐伯市|豊後大野市"))
                        respData = "大分県南部";
                    if (Regex.IsMatch(point.addr, "日田市|竹田市|九重町|玖珠町"))
                        respData = "大分県西部";
                    break;
                case "宮崎県":
                    if (Regex.IsMatch(point.addr, "延岡市|日向市|西都市|高鍋町|新富町|木城町|川南町|宮崎都農町|門川町"))
                        respData = "宮崎県北部平野部";
                    if (Regex.IsMatch(point.addr, "西米良村|諸塚村|椎葉村|宮崎美郷町|高千穂町|日之影町|五ヶ瀬町"))
                        respData = "宮崎県北部山沿い";
                    if (Regex.IsMatch(point.addr, "宮崎市|日南市|串間市|国富町|綾町"))
                        respData = "宮崎県南部平野部";
                    if (Regex.IsMatch(point.addr, "都城市|小林市|えびの市|三股町|高原町"))
                        respData = "宮崎県南部山沿い";
                    break;
                case "鹿児島県":
                    if (Regex.IsMatch(point.addr, "鹿児島市|枕崎市|阿久根市|鹿児島出水市|指宿市|薩摩川内市|日置市|霧島市|いちき串木野市|南さつま市|南九州市|伊佐市|姶良市|さつま町|長島町|湧水町"))
                        respData = "鹿児島県薩摩";
                    if (Regex.IsMatch(point.addr, "鹿屋市|垂水市|曽於市|志布志市|大崎町|東串良町|錦江町|南大隅町|肝付町"))
                        respData = "鹿児島県大隅";
                    if (Regex.IsMatch(point.addr, "鹿児島十島村"))
                        respData = "鹿児島県十島村";
                    if (Regex.IsMatch(point.addr, "薩摩川内市甑島"))
                        respData = "鹿児島県甑島";
                    if (Regex.IsMatch(point.addr, "西之表市|三島村|中種子町|南種子町"))
                        respData = "鹿児島県種子島";
                    if (Regex.IsMatch(point.addr, "屋久島町"))
                        respData = "鹿児島県屋久島";
                    if (Regex.IsMatch(point.addr, "奄美市|大和村|宇検村|瀬戸内町|龍郷町|喜界町"))
                        respData = "鹿児島県奄美北部";
                    if (Regex.IsMatch(point.addr, "徳之島町|天城町|伊仙町|和泊町|知名町|与論町"))
                        respData = "鹿児島県奄美南部";
                    break;
                case "沖縄県":
                    if (Regex.IsMatch(point.addr, "名護市|国頭村|大宜味村|東村|今帰仁村|本部町|恩納村|宜野座村|金武町|伊江村|粟国村|伊平屋村|伊是名村"))
                        respData = "沖縄県本島北部";
                    if (Regex.IsMatch(point.addr, "那覇市|宜野湾市|浦添市|糸満市|沖縄市|豊見城市|うるま市|南城市|読谷村|嘉手納町|北谷町|北中城村|中城村|西原町|与那原町|南風原町|渡嘉敷村|座間味村|渡名喜村|八重瀬町"))
                        respData = "沖縄県本島中南部";
                    if (Regex.IsMatch(point.addr, "久米島町"))
                        respData = "沖縄県久米島";
                    if (Regex.IsMatch(point.addr, "南大東村|北大東村"))
                        respData = "沖縄県大東島";
                    if (Regex.IsMatch(point.addr, "宮古島市|多良間村"))
                        respData = "沖縄県宮古島";
                    if (Regex.IsMatch(point.addr, "石垣市"))
                        respData = "沖縄県石垣島";
                    if (Regex.IsMatch(point.addr, "与那国町"))
                        respData = "沖縄県与那国島";
                    if (Regex.IsMatch(point.addr, "竹富町"))
                        respData = "沖縄県西表島";
                    break;
            }
            return respData;
        }

        public string IntenToShindo(int maxint)
        {
            
            switch (maxint)
            {
                case -1:
                    return "不明";
                case 10:
                    return "１";
                case 20:
                    return "２";
                case 30:
                    return "３";
                case 40:
                    return "４";
                case 45:
                    return "５弱";
                case 46:
                    return "５弱以上未入電";
                case 50:
                    return "５強";
                case 55:
                    return "６弱";
                case 60:
                    return "６強";
                case 70:
                    return "７";
                default:
                    return "不明";
            }
        }

        public class P2PEqAPI
        {
            public int code { get; set; }
            public Earthquake earthquake { get; set; }
            public string id { get; set; }
            public Issue issue { get; set; }
            public List<Point> points { get; set; }
            public string time { get; set; }

        }

        public class Earthquake
        {
            public string domesticTsunami { get; set; }
            public string foreignTsunami { get; set; }
            public Hypocenter hypocenter { get; set; }
            public int maxScale { get; set; }
            public string time { get; set; }
        }

        public class Hypocenter
        {
            public int depth { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double magnitude { get; set; }
            public string name { get; set; }
        }

        public class Issue
        {
            public string correct { get; set; }
            public string source { get; set; }
            public string time { get; set; }
            public string type { get; set; }
        }

        public class Point
        {
            public string addr { get; set; }
            public bool isArea { get; set; }
            public string pref { get; set; }
            public int scale { get; set; }
        }

    }
}
