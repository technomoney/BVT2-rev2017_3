using UnityEngine;
using System.Collections.Generic;
using HeathenEngineering.UIX.Serialization;
using System.Text;

namespace HeathenEngineering.UIX
{
    [CreateAssetMenu(menuName = "Library Variables/Ligature Library")]
    public class LigatureLibrary : ScriptableObject
    {
        /// <summary>
        /// List of ligatures to test for
        /// </summary>
        public List<LigatureReference> map = new List<LigatureReference>();
        
        public string ParseEnd(string value)
        {
            StringBuilder working = new StringBuilder(value);
            if (map != null)
            {
                foreach (LigatureReference lig in map)
                {
                    if (value.EndsWith(lig.Characters))
                    {
                        return value.Substring(0, value.Length - lig.Characters.Length) + lig.Ligature;
                    }
                    else
                        return value;
                }
            }

            return working.ToString();
        }

        public string ParseAll(string value)
        {
            StringBuilder working = new StringBuilder(value);
            if (map != null)
            {
                foreach (LigatureReference lig in map)
                {
                    working = working.Replace(lig.Characters, lig.Ligature);
                }
            }

            return working.ToString();
        }

        [ContextMenu("Add Common Ligatures")]
        public void AddCommonLigatures()
        {
            if (map == null)
                map = new List<LigatureReference>();

            map.Add(new LigatureReference("(C)", "©"));
            map.Add(new LigatureReference("(c)", "©"));
            map.Add(new LigatureReference("(SC)", "℗"));
            map.Add(new LigatureReference("(sc)", "℗"));
            map.Add(new LigatureReference("(Sc)", "℗"));
            map.Add(new LigatureReference("(sC)", "℗"));
            map.Add(new LigatureReference("(TM)", "™"));
            map.Add(new LigatureReference("(tm)", "™"));
            map.Add(new LigatureReference("(Tm)", "™"));
            map.Add(new LigatureReference("(tM)", "™"));
            map.Add(new LigatureReference("(RTM)", "®"));
            map.Add(new LigatureReference("(rtm)", "®"));
            map.Add(new LigatureReference("(rTm)", "®"));
            map.Add(new LigatureReference("(Rtm)", "®"));
            map.Add(new LigatureReference("(RTm)", "®"));
            map.Add(new LigatureReference("(RtM)", "®"));
            map.Add(new LigatureReference("(rTM)", "®"));
            map.Add(new LigatureReference("(rtM)", "®"));
            map.Add(new LigatureReference("(sm)", "℠"));
            map.Add(new LigatureReference("(SM)", "℠"));
            map.Add(new LigatureReference("(Sm)", "℠"));
            map.Add(new LigatureReference("(sM)", "℠"));
            map.Add(new LigatureReference("`a", "à"));
            map.Add(new LigatureReference("`A", "À"));
            map.Add(new LigatureReference("`e", "è"));
            map.Add(new LigatureReference("`E", "È"));
            map.Add(new LigatureReference("`i", "ì"));
            map.Add(new LigatureReference("`I", "Ì"));
            map.Add(new LigatureReference("`o", "ò"));
            map.Add(new LigatureReference("`O", "Ò"));
            map.Add(new LigatureReference("`u", "ù"));
            map.Add(new LigatureReference("`U", "Ù"));
            map.Add(new LigatureReference("(No)", "№"));
            map.Add(new LigatureReference("(#)", "№"));
            map.Add(new LigatureReference("(deg)", "°"));
        }

        [ContextMenu("Add Supper Script Ligatures")]
        public void AddSupperScriptLigatures()
        {
            if (map == null)
                map = new List<LigatureReference>();

            map.Add(new LigatureReference("^1", "¹"));
            map.Add(new LigatureReference("^2", "²"));
            map.Add(new LigatureReference("^3", "³"));
            map.Add(new LigatureReference("^4", "⁴"));
            map.Add(new LigatureReference("^5", "⁵"));
            map.Add(new LigatureReference("^6", "⁶"));
            map.Add(new LigatureReference("^7", "⁷"));
            map.Add(new LigatureReference("^8", "⁸"));
            map.Add(new LigatureReference("^9", "⁹"));
            map.Add(new LigatureReference("^0", "⁰"));
            map.Add(new LigatureReference("^+", "⁺"));
            map.Add(new LigatureReference("^-", "⁻"));
            map.Add(new LigatureReference("^=", "⁼"));
            map.Add(new LigatureReference("^(", "⁽"));
            map.Add(new LigatureReference("^)", "⁾"));
            map.Add(new LigatureReference("^a", "ᵃ"));
            map.Add(new LigatureReference("^b", "ᵇ"));
            map.Add(new LigatureReference("^c", "ᶜ"));
            map.Add(new LigatureReference("^d", "ᵈ"));
            map.Add(new LigatureReference("^e", "ᵉ"));
            map.Add(new LigatureReference("^f", "ᶠ"));
            map.Add(new LigatureReference("^g", "ᵍ"));
            map.Add(new LigatureReference("^h", "ʰ"));
            map.Add(new LigatureReference("^i", "ⁱ"));
            map.Add(new LigatureReference("^j", "ʲ"));
            map.Add(new LigatureReference("^k", "ᵏ"));
            map.Add(new LigatureReference("^l", "ˡ"));
            map.Add(new LigatureReference("^m", "ᵐ"));
            map.Add(new LigatureReference("^n", "ⁿ"));
            map.Add(new LigatureReference("^o", "ᵒ"));
            map.Add(new LigatureReference("^p", "ᵖ"));
            map.Add(new LigatureReference("^r", "ʳ"));
            map.Add(new LigatureReference("^s", "ˢ"));
            map.Add(new LigatureReference("^t", "ᵗ"));
            map.Add(new LigatureReference("^u", "ᵘ"));
            map.Add(new LigatureReference("^v", "ᵛ"));
            map.Add(new LigatureReference("^w", "ʷ"));
            map.Add(new LigatureReference("^x", "ˣ"));
            map.Add(new LigatureReference("^y", "ʸ"));
            map.Add(new LigatureReference("^z", "ᶻ"));
            map.Add(new LigatureReference("^A", "ᴬ"));
            map.Add(new LigatureReference("^B", "ᴮ"));
            map.Add(new LigatureReference("^D", "ᴰ"));
            map.Add(new LigatureReference("^E", "ᴱ"));
            map.Add(new LigatureReference("^G", "ᴳ"));
            map.Add(new LigatureReference("^H", "ᴴ"));
            map.Add(new LigatureReference("^I", "ᴵ"));
            map.Add(new LigatureReference("^J", "ᴶ"));
            map.Add(new LigatureReference("^K", "ᴷ"));
            map.Add(new LigatureReference("^L", "ᴸ"));
            map.Add(new LigatureReference("^M", "ᴹ"));
            map.Add(new LigatureReference("^N", "ᴺ"));
            map.Add(new LigatureReference("^O", "ᴼ"));
            map.Add(new LigatureReference("^P", "ᴾ"));
            map.Add(new LigatureReference("^R", "ᴿ"));
            map.Add(new LigatureReference("^T", "ᵀ"));
            map.Add(new LigatureReference("^U", "ᵁ"));
            map.Add(new LigatureReference("^V", "ⱽ"));
            map.Add(new LigatureReference("^W", "ᵂ"));
        }

        [ContextMenu("Add Fraction Ligatures")]
        public void AddFractionLigatures()
        {
            if (map == null)
                map = new List<LigatureReference>();

            map.Add(new LigatureReference("1/4", "¼"));
            map.Add(new LigatureReference("2/4", "½"));
            map.Add(new LigatureReference("1/2", "½"));
            map.Add(new LigatureReference("3/4", "¾"));
            map.Add(new LigatureReference("1/10", "⅒"));
            map.Add(new LigatureReference("2/10", "⅕"));
            map.Add(new LigatureReference("4/10", "⅖"));
            map.Add(new LigatureReference("5/10", "½"));
            map.Add(new LigatureReference("6/10", "⅗"));
            map.Add(new LigatureReference("8/10", "⅘"));
            map.Add(new LigatureReference("1/3", "⅓"));
            map.Add(new LigatureReference("2/3", "⅔"));
            map.Add(new LigatureReference("1/5", "⅕"));
            map.Add(new LigatureReference("2/5", "⅖"));
            map.Add(new LigatureReference("3/5", "⅗"));
            map.Add(new LigatureReference("4/5", "⅘"));
            map.Add(new LigatureReference("1/6", "⅙"));
            map.Add(new LigatureReference("2/6", "⅓"));
            map.Add(new LigatureReference("3/6", "½"));
            map.Add(new LigatureReference("4/6", "⅔"));
            map.Add(new LigatureReference("5/6", "⅚"));
            map.Add(new LigatureReference("1/8", "⅛"));
            map.Add(new LigatureReference("2/8", "¼"));
            map.Add(new LigatureReference("3/8", "⅜"));
            map.Add(new LigatureReference("4/8", "½"));
            map.Add(new LigatureReference("5/8", "⅝"));
            map.Add(new LigatureReference("6/8", "¾"));
            map.Add(new LigatureReference("7/8", "⅞"));
        }

        [ContextMenu("Add Katakana カタカナ Ligatures")]
        public void AddKatakanaLigatures()
        {
            if (map == null)
                map = new List<LigatureReference>();

            //A
            map.Add(new LigatureReference("ア=", "ァ"));
            map.Add(new LigatureReference("ア-", "ァ"));
            map.Add(new LigatureReference("ka", "カ"));
            map.Add(new LigatureReference("sa", "サ"));
            map.Add(new LigatureReference("ta", "タ"));
            map.Add(new LigatureReference("na", "ナ"));
            map.Add(new LigatureReference("ha", "ハ"));
            map.Add(new LigatureReference("ma", "マ"));
            map.Add(new LigatureReference("ya", "ヤ"));
            map.Add(new LigatureReference("ヤ=", "ャ"));
            map.Add(new LigatureReference("ヤ-", "ャ"));
            map.Add(new LigatureReference("ra", "ラ"));
            map.Add(new LigatureReference("wa", "ワ"));
            map.Add(new LigatureReference("ga", "ガ"));
            map.Add(new LigatureReference("za", "ザ"));
            map.Add(new LigatureReference("da", "ダ"));
            map.Add(new LigatureReference("ba", "バ"));
            map.Add(new LigatureReference("pa", "パ"));
            map.Add(new LigatureReference("ンa", "ナ"));
            map.Add(new LigatureReference("a", "ア"));
            //I
            map.Add(new LigatureReference("イ=", "ィ"));
            map.Add(new LigatureReference("イ-", "ィ"));
            map.Add(new LigatureReference("ki", "キ"));
            map.Add(new LigatureReference("shi", "シ"));
            map.Add(new LigatureReference("chi", "チ"));
            map.Add(new LigatureReference("ni", "ニ"));
            map.Add(new LigatureReference("hi", "ヒ"));
            map.Add(new LigatureReference("mi", "ミ"));
            map.Add(new LigatureReference("ri", "リ"));
            map.Add(new LigatureReference("wi", "ヰ"));
            map.Add(new LigatureReference("gi", "ギ"));
            map.Add(new LigatureReference("ji", "ジ"));
            map.Add(new LigatureReference("di", "ヂ"));
            map.Add(new LigatureReference("ジ=", "ヂ"));
            map.Add(new LigatureReference("ジ-", "ヂ"));
            map.Add(new LigatureReference("bi", "ビ"));
            map.Add(new LigatureReference("pi", "ピ"));
            map.Add(new LigatureReference("i", "イ"));
            //U
            map.Add(new LigatureReference("ウ=", "ゥ"));
            map.Add(new LigatureReference("ウ-", "ゥ"));
            map.Add(new LigatureReference("ku", "ク"));
            map.Add(new LigatureReference("tス", "ツ"));
            map.Add(new LigatureReference("tsu", "ツ"));
            map.Add(new LigatureReference("su", "ス"));
            map.Add(new LigatureReference("nu", "ヌ"));
            map.Add(new LigatureReference("fu", "フ"));
            map.Add(new LigatureReference("mu", "ム"));
            map.Add(new LigatureReference("yu", "ユ"));
            map.Add(new LigatureReference("ユ=", "ュ"));
            map.Add(new LigatureReference("ユ-", "ュ"));
            map.Add(new LigatureReference("ru", "ル"));
            map.Add(new LigatureReference("gu", "グ"));
            map.Add(new LigatureReference("zu", "ズ"));
            map.Add(new LigatureReference("du", "ヅ"));
            map.Add(new LigatureReference("ズ=", "ヅ"));
            map.Add(new LigatureReference("ズ-", "ヅ"));
            map.Add(new LigatureReference("bu", "ブ"));
            map.Add(new LigatureReference("bu", "プ"));
            map.Add(new LigatureReference("u", "ウ"));
            //E
            map.Add(new LigatureReference("エ=", "ェ"));
            map.Add(new LigatureReference("エ-", "ェ"));
            map.Add(new LigatureReference("ke", "ケ"));
            map.Add(new LigatureReference("se", "セ"));
            map.Add(new LigatureReference("te", "テ"));
            map.Add(new LigatureReference("ne", "ネ"));
            map.Add(new LigatureReference("he", "ヘ"));
            map.Add(new LigatureReference("me", "メ"));
            map.Add(new LigatureReference("re", "レ"));
            map.Add(new LigatureReference("we", "ヱ"));
            map.Add(new LigatureReference("ge", "ゲ"));
            map.Add(new LigatureReference("ze", "ゼ"));
            map.Add(new LigatureReference("de", "デ"));
            map.Add(new LigatureReference("be", "ベ"));
            map.Add(new LigatureReference("pe", "ペ"));
            map.Add(new LigatureReference("e", "エ"));
            //O
            map.Add(new LigatureReference("オ=", "ォ"));
            map.Add(new LigatureReference("オ-", "ォ"));
            map.Add(new LigatureReference("ko", "コ"));
            map.Add(new LigatureReference("so", "ソ"));
            map.Add(new LigatureReference("to", "ト"));
            map.Add(new LigatureReference("no", "ノ"));
            map.Add(new LigatureReference("ho", "ホ"));
            map.Add(new LigatureReference("mo", "モ"));
            map.Add(new LigatureReference("yo", "ヨ"));
            map.Add(new LigatureReference("ヨ=", "ョ"));
            map.Add(new LigatureReference("ヨ-", "ョ"));
            map.Add(new LigatureReference("ro", "ロ"));
            map.Add(new LigatureReference("wo", "ヲ"));
            map.Add(new LigatureReference("go", "ゴ"));
            map.Add(new LigatureReference("zo", "ゾ"));
            map.Add(new LigatureReference("do", "ド"));
            map.Add(new LigatureReference("bo", "ボ"));
            map.Add(new LigatureReference("po", "ポ"));
            map.Add(new LigatureReference("o", "オ"));
            //Alt
            map.Add(new LigatureReference("v", "ヴ"));
            map.Add(new LigatureReference("n", "ン"));
        }

        [ContextMenu("Add Korean 한국어 Ligatures")]
        public void AddKoreanLigatures()
        {
            if (map == null)
                map = new List<LigatureReference>();

            string[] set1 = { "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ", "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ" };
    string[] set2 = { "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };
    string[][] ligSet = {   new string[]{"가", "까", "나", "다", "따", "라", "마", "바", "빠", "사", "싸", "아", "자", "짜", "차", "카", "타", "파", "하"},
                                    new string[]{"개", "깨", "내", "대", "때", "래", "매", "배", "빼", "새", "쌔", "애", "재", "째", "채", "캐", "태", "패", "해"},
                                    new string[]{"갸", "꺄", "냐", "댜", "땨", "랴", "먀", "뱌", "뺘", "샤", "쌰", "야", "쟈", "쨔", "챠", "캬", "탸", "퍄", "햐"},
                                    new string[]{"걔", "꺠", "냬", "댸", "떄", "럐", "먜", "뱨", "뺴", "섀", "썌", "얘", "쟤", "쨰", "챼", "컈", "턔", "퍠", "햬"},
                                    new string[]{"거", "꺼", "너", "더", "떠", "러", "머", "버", "뻐", "서", "써", "어", "저", "쩌", "처", "커", "터", "퍼", "허"},
                                    new string[]{"게", "께", "네", "데", "떼", "레", "메", "베", "뻬", "세", "쎄", "에", "제", "쩨", "체", "케", "테", "페", "헤"},
                                    new string[]{"겨", "껴", "녀", "뎌", "뗘", "려", "며", "벼", "뼈", "셔", "쎠", "여", "져", "쪄", "쳐", "켜", "텨", "폐", "혀"},
                                    new string[]{"계", "꼐", "녜", "뎨", "뗴", "례", "몌", "볘", "뼤", "셰", "쎼", "예", "졔", "쪠", "쳬", "켸", "톄", "폐", "혜"},
                                    new string[]{"고", "	꼬", "노", "도", "또", "로", "모", "보", "뽀", "소", "쏘", "오", "조", "쪼", "초", "코", "토", "포", "호"},
                                    new string[]{"과", "꽈", "놔", "돠", "똬", "롸", "뫄", "봐", "뽜", "솨", "쏴", "와", "좌", "쫘", "촤", "콰", "톼", "퐈", "화"},
                                    new string[]{"괘", "꽤", "놰", "돼", "뙈", "뢔", "뫠", "봬", "뽸", "쇄", "쐐", "왜", "좨", "쫴", "쵀", "쾌", "퇘", "퐤", "홰"},
                                    new string[]{"괴", "꾀", "뇌", "되", "뙤", "뢰", "뫼", "뵈", "뾔", "쇠", "쐬", "외", "죄", "쬐", "최", "쾨", "퇴", "푀", "회"},
                                    new string[]{"교", "꾜", "뇨", "됴", "뚀", "료", "묘", "뵤", "뾰", "쇼", "쑈", "요", "죠", "쬬", "쵸", "쿄", "툐", "표", "효"},
                                    new string[]{"구", "꾸", "누", "두", "뚜", "루", "무", "부", "뿌", "수", "쑤", "우", "주", "쭈", "추", "쿠", "투", "푸", "후"},
                                    new string[]{"궈", "꿔", "눠", "둬", "뚸", "뤄", "뭐", "붜", "뿨", "숴", "쒀", "워", "줘", "쭤", "춰", "쿼", "퉈", "풔", "훠"},
                                    new string[]{"궤", "꿰", "눼", "뒈", "뛔", "뤠", "뭬", "붸", "쀄", "쉐", "쒜", "웨", "줴", "쮀", "췌", "퀘", "퉤", "풰", "훼"},
                                    new string[]{"귀", "뀌", "뉘", "뒤", "뛰", "뤼", "뮈", "뷔", "쀠", "쉬", "쒸", "위", "쥐", "쮜", "취", "퀴", "튀", "퓌", "휘"},
                                    new string[]{"규", "뀨", "뉴", "듀", "뜌", "류", "뮤", "뷰", "쀼", "슈", "쓔", "유", "쥬", "쮸", "츄", "큐", "튜", "퓨", "휴"},
                                    new string[]{"그", "끄", "느", "드", "뜨", "르", "므", "브", "쁘", "스", "쓰", "으", "즈", "쯔", "츠", "크", "트", "프", "흐"},
                                    new string[]{"긔", "끠", "늬", "듸", "띄", "릐", "믜", "븨", "쁴", "싀", "씌", "의", "즤", "쯰", "츼", "킈", "틔", "픠", "희"},
                                    new string[]{"기", "끼", "니", "디", "띠", "리", "미", "비", "삐", "시", "씨", "이", "지", "찌", "치", "키", "티", "피", "히" }};

            for (int i = 0; i<set2.Length; i++)
            {
                for (int ii = 0; ii<set1.Length; ii++)
                {
                    map.Add(new LigatureReference(set2[i] + set1[ii], ligSet[ii][i]));
                }
            }
        }
    }
}