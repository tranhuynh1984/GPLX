﻿/*! URI.js v1.19.7 http://medialize.github.io/URI.js/ */
/* build contains: IPv6.js, punycode.js, SecondLevelDomains.js, URI.js, URITemplate.js, jquery.URI.js, URI.fragmentQuery.js */
/*
 URI.js - Mutating URLs
 IPv6 Support

 Version: 1.19.7

 Author: Rodney Rehm
 Web: http://medialize.github.io/URI.js/

 Licensed under
   MIT License http://www.opensource.org/licenses/mit-license

 https://mths.be/punycode v1.4.0 by @mathias  URI.js - Mutating URLs
 Second Level Domain (SLD) Support

 Version: 1.19.7

 Author: Rodney Rehm
 Web: http://medialize.github.io/URI.js/

 Licensed under
   MIT License http://www.opensource.org/licenses/mit-license

 URI.js - Mutating URLs

 Version: 1.19.7

 Author: Rodney Rehm
 Web: http://medialize.github.io/URI.js/

 Licensed under
   MIT License http://www.opensource.org/licenses/mit-license

 URI.js - Mutating URLs
 URI Template Support - http://tools.ietf.org/html/rfc6570

 Version: 1.19.7

 Author: Rodney Rehm
 Web: http://medialize.github.io/URI.js/

 Licensed under
   MIT License http://www.opensource.org/licenses/mit-license

 URI.js - Mutating URLs
 jQuery Plugin

 Version: 1.19.7

 Author: Rodney Rehm
 Web: http://medialize.github.io/URI.js/jquery-uri-plugin.html

 Licensed under
   MIT License http://www.opensource.org/licenses/mit-license

*/
(function (k, p) { "object" === typeof module && module.exports ? module.exports = p() : "function" === typeof define && define.amd ? define(p) : k.IPv6 = p(k) })(this, function (k) {
    var p = k && k.IPv6; return {
        best: function (m) {
            m = m.toLowerCase().split(":"); var r = m.length, d = 8; "" === m[0] && "" === m[1] && "" === m[2] ? (m.shift(), m.shift()) : "" === m[0] && "" === m[1] ? m.shift() : "" === m[r - 1] && "" === m[r - 2] && m.pop(); r = m.length; -1 !== m[r - 1].indexOf(".") && (d = 7); var q; for (q = 0; q < r && "" !== m[q]; q++); if (q < d) for (m.splice(q, 1, "0000"); m.length < d;)m.splice(q, 0, "0000");
            for (q = 0; q < d; q++) { r = m[q].split(""); for (var y = 0; 3 > y; y++)if ("0" === r[0] && 1 < r.length) r.splice(0, 1); else break; m[q] = r.join("") } r = -1; var x = y = 0, l = -1, u = !1; for (q = 0; q < d; q++)u ? "0" === m[q] ? x += 1 : (u = !1, x > y && (r = l, y = x)) : "0" === m[q] && (u = !0, l = q, x = 1); x > y && (r = l, y = x); 1 < y && m.splice(r, y, ""); r = m.length; d = ""; "" === m[0] && (d = ":"); for (q = 0; q < r; q++) { d += m[q]; if (q === r - 1) break; d += ":" } "" === m[r - 1] && (d += ":"); return d
        }, noConflict: function () { k.IPv6 === this && (k.IPv6 = p); return this }
    }
});
(function (k) {
    function p(n) { throw new RangeError(C[n]); } function m(n, A) { for (var H = n.length, D = []; H--;)D[H] = A(n[H]); return D } function r(n, A) { var H = n.split("@"), D = ""; 1 < H.length && (D = H[0] + "@", n = H[1]); n = n.replace(z, "."); H = n.split("."); H = m(H, A).join("."); return D + H } function d(n) { for (var A = [], H = 0, D = n.length, K, M; H < D;)K = n.charCodeAt(H++), 55296 <= K && 56319 >= K && H < D ? (M = n.charCodeAt(H++), 56320 == (M & 64512) ? A.push(((K & 1023) << 10) + (M & 1023) + 65536) : (A.push(K), H--)) : A.push(K); return A } function q(n) {
        return m(n, function (A) {
            var H =
                ""; 65535 < A && (A -= 65536, H += g(A >>> 10 & 1023 | 55296), A = 56320 | A & 1023); return H += g(A)
        }).join("")
    } function y(n, A, H) { var D = 0; n = H ? B(n / 700) : n >> 1; for (n += B(n / A); 455 < n; D += 36)n = B(n / 35); return B(D + 36 * n / (n + 38)) } function x(n) {
        var A = [], H = n.length, D = 0, K = 128, M = 72, a, b; var c = n.lastIndexOf("-"); 0 > c && (c = 0); for (a = 0; a < c; ++a)128 <= n.charCodeAt(a) && p("not-basic"), A.push(n.charCodeAt(a)); for (c = 0 < c ? c + 1 : 0; c < H;) {
            a = D; var e = 1; for (b = 36; ; b += 36) {
                c >= H && p("invalid-input"); var f = n.charCodeAt(c++); f = 10 > f - 48 ? f - 22 : 26 > f - 65 ? f - 65 : 26 > f - 97 ? f - 97 : 36;
                (36 <= f || f > B((2147483647 - D) / e)) && p("overflow"); D += f * e; var v = b <= M ? 1 : b >= M + 26 ? 26 : b - M; if (f < v) break; f = 36 - v; e > B(2147483647 / f) && p("overflow"); e *= f
            } e = A.length + 1; M = y(D - a, e, 0 == a); B(D / e) > 2147483647 - K && p("overflow"); K += B(D / e); D %= e; A.splice(D++, 0, K)
        } return q(A)
    } function l(n) {
        var A, H, D, K = []; n = d(n); var M = n.length; var a = 128; var b = 0; var c = 72; for (D = 0; D < M; ++D) { var e = n[D]; 128 > e && K.push(g(e)) } for ((A = H = K.length) && K.push("-"); A < M;) {
            var f = 2147483647; for (D = 0; D < M; ++D)e = n[D], e >= a && e < f && (f = e); var v = A + 1; f - a > B((2147483647 - b) / v) &&
                p("overflow"); b += (f - a) * v; a = f; for (D = 0; D < M; ++D)if (e = n[D], e < a && 2147483647 < ++b && p("overflow"), e == a) { var E = b; for (f = 36; ; f += 36) { e = f <= c ? 1 : f >= c + 26 ? 26 : f - c; if (E < e) break; var J = E - e; E = 36 - e; var L = K; e += J % E; L.push.call(L, g(e + 22 + 75 * (26 > e) - 0)); E = B(J / E) } K.push(g(E + 22 + 75 * (26 > E) - 0)); c = y(b, v, A == H); b = 0; ++A } ++b; ++a
        } return K.join("")
    } var u = "object" == typeof exports && exports && !exports.nodeType && exports, F = "object" == typeof module && module && !module.nodeType && module, h = "object" == typeof global && global; if (h.global === h || h.window === h ||
        h.self === h) k = h; var t = /^xn--/, w = /[^\x20-\x7E]/, z = /[\x2E\u3002\uFF0E\uFF61]/g, C = { overflow: "Overflow: input needs wider integers to process", "not-basic": "Illegal input >= 0x80 (not a basic code point)", "invalid-input": "Invalid input" }, B = Math.floor, g = String.fromCharCode, G; var I = {
            version: "1.3.2", ucs2: { decode: d, encode: q }, decode: x, encode: l, toASCII: function (n) { return r(n, function (A) { return w.test(A) ? "xn--" + l(A) : A }) }, toUnicode: function (n) {
                return r(n, function (A) {
                    return t.test(A) ? x(A.slice(4).toLowerCase()) :
                        A
                })
            }
        }; if ("function" == typeof define && "object" == typeof define.amd && define.amd) define("punycode", function () { return I }); else if (u && F) if (module.exports == u) F.exports = I; else for (G in I) I.hasOwnProperty(G) && (u[G] = I[G]); else k.punycode = I
})(this);
(function (k, p) { "object" === typeof module && module.exports ? module.exports = p() : "function" === typeof define && define.amd ? define(p) : k.SecondLevelDomains = p(k) })(this, function (k) {
    var p = k && k.SecondLevelDomains, m = {
        list: {
            ac: " com gov mil net org ", ae: " ac co gov mil name net org pro sch ", af: " com edu gov net org ", al: " com edu gov mil net org ", ao: " co ed gv it og pb ", ar: " com edu gob gov int mil net org tur ", at: " ac co gv or ", au: " asn com csiro edu gov id net org ", ba: " co com edu gov mil net org rs unbi unmo unsa untz unze ",
            bb: " biz co com edu gov info net org store tv ", bh: " biz cc com edu gov info net org ", bn: " com edu gov net org ", bo: " com edu gob gov int mil net org tv ", br: " adm adv agr am arq art ato b bio blog bmd cim cng cnt com coop ecn edu eng esp etc eti far flog fm fnd fot fst g12 ggf gov imb ind inf jor jus lel mat med mil mus net nom not ntr odo org ppg pro psc psi qsl rec slg srv tmp trd tur tv vet vlog wiki zlg ", bs: " com edu gov net org ", bz: " du et om ov rg ", ca: " ab bc mb nb nf nl ns nt nu on pe qc sk yk ",
            ck: " biz co edu gen gov info net org ", cn: " ac ah bj com cq edu fj gd gov gs gx gz ha hb he hi hl hn jl js jx ln mil net nm nx org qh sc sd sh sn sx tj tw xj xz yn zj ", co: " com edu gov mil net nom org ", cr: " ac c co ed fi go or sa ", cy: " ac biz com ekloges gov ltd name net org parliament press pro tm ", "do": " art com edu gob gov mil net org sld web ", dz: " art asso com edu gov net org pol ", ec: " com edu fin gov info med mil net org pro ", eg: " com edu eun gov mil name net org sci ", er: " com edu gov ind mil net org rochest w ",
            es: " com edu gob nom org ", et: " biz com edu gov info name net org ", fj: " ac biz com info mil name net org pro ", fk: " ac co gov net nom org ", fr: " asso com f gouv nom prd presse tm ", gg: " co net org ", gh: " com edu gov mil org ", gn: " ac com gov net org ", gr: " com edu gov mil net org ", gt: " com edu gob ind mil net org ", gu: " com edu gov net org ", hk: " com edu gov idv net org ", hu: " 2000 agrar bolt casino city co erotica erotika film forum games hotel info ingatlan jogasz konyvelo lakas media news org priv reklam sex shop sport suli szex tm tozsde utazas video ",
            id: " ac co go mil net or sch web ", il: " ac co gov idf k12 muni net org ", "in": " ac co edu ernet firm gen gov i ind mil net nic org res ", iq: " com edu gov i mil net org ", ir: " ac co dnssec gov i id net org sch ", it: " edu gov ", je: " co net org ", jo: " com edu gov mil name net org sch ", jp: " ac ad co ed go gr lg ne or ", ke: " ac co go info me mobi ne or sc ", kh: " com edu gov mil net org per ", ki: " biz com de edu gov info mob net org tel ", km: " asso com coop edu gouv k medecin mil nom notaires pharmaciens presse tm veterinaire ",
            kn: " edu gov net org ", kr: " ac busan chungbuk chungnam co daegu daejeon es gangwon go gwangju gyeongbuk gyeonggi gyeongnam hs incheon jeju jeonbuk jeonnam k kg mil ms ne or pe re sc seoul ulsan ", kw: " com edu gov net org ", ky: " com edu gov net org ", kz: " com edu gov mil net org ", lb: " com edu gov net org ", lk: " assn com edu gov grp hotel int ltd net ngo org sch soc web ", lr: " com edu gov net org ", lv: " asn com conf edu gov id mil net org ", ly: " com edu gov id med net org plc sch ", ma: " ac co gov m net org press ",
            mc: " asso tm ", me: " ac co edu gov its net org priv ", mg: " com edu gov mil nom org prd tm ", mk: " com edu gov inf name net org pro ", ml: " com edu gov net org presse ", mn: " edu gov org ", mo: " com edu gov net org ", mt: " com edu gov net org ", mv: " aero biz com coop edu gov info int mil museum name net org pro ", mw: " ac co com coop edu gov int museum net org ", mx: " com edu gob net org ", my: " com edu gov mil name net org sch ", nf: " arts com firm info net other per rec store web ", ng: " biz com edu gov mil mobi name net org sch ",
            ni: " ac co com edu gob mil net nom org ", np: " com edu gov mil net org ", nr: " biz com edu gov info net org ", om: " ac biz co com edu gov med mil museum net org pro sch ", pe: " com edu gob mil net nom org sld ", ph: " com edu gov i mil net ngo org ", pk: " biz com edu fam gob gok gon gop gos gov net org web ", pl: " art bialystok biz com edu gda gdansk gorzow gov info katowice krakow lodz lublin mil net ngo olsztyn org poznan pwr radom slupsk szczecin torun warszawa waw wroc wroclaw zgora ", pr: " ac biz com edu est gov info isla name net org pro prof ",
            ps: " com edu gov net org plo sec ", pw: " belau co ed go ne or ", ro: " arts com firm info nom nt org rec store tm www ", rs: " ac co edu gov in org ", sb: " com edu gov net org ", sc: " com edu gov net org ", sh: " co com edu gov net nom org ", sl: " com edu gov net org ", st: " co com consulado edu embaixada gov mil net org principe saotome store ", sv: " com edu gob org red ", sz: " ac co org ", tr: " av bbs bel biz com dr edu gen gov info k12 name net org pol tel tsk tv web ", tt: " aero biz cat co com coop edu gov info int jobs mil mobi museum name net org pro tel travel ",
            tw: " club com ebiz edu game gov idv mil net org ", mu: " ac co com gov net or org ", mz: " ac co edu gov org ", na: " co com ", nz: " ac co cri geek gen govt health iwi maori mil net org parliament school ", pa: " abo ac com edu gob ing med net nom org sld ", pt: " com edu gov int net nome org publ ", py: " com edu gov mil net org ", qa: " com edu gov mil net org ", re: " asso com nom ", ru: " ac adygeya altai amur arkhangelsk astrakhan bashkiria belgorod bir bryansk buryatia cbg chel chelyabinsk chita chukotka chuvashia com dagestan e-burg edu gov grozny int irkutsk ivanovo izhevsk jar joshkar-ola kalmykia kaluga kamchatka karelia kazan kchr kemerovo khabarovsk khakassia khv kirov koenig komi kostroma kranoyarsk kuban kurgan kursk lipetsk magadan mari mari-el marine mil mordovia mosreg msk murmansk nalchik net nnov nov novosibirsk nsk omsk orenburg org oryol penza perm pp pskov ptz rnd ryazan sakhalin samara saratov simbirsk smolensk spb stavropol stv surgut tambov tatarstan tom tomsk tsaritsyn tsk tula tuva tver tyumen udm udmurtia ulan-ude vladikavkaz vladimir vladivostok volgograd vologda voronezh vrn vyatka yakutia yamal yekaterinburg yuzhno-sakhalinsk ",
            rw: " ac co com edu gouv gov int mil net ", sa: " com edu gov med net org pub sch ", sd: " com edu gov info med net org tv ", se: " a ac b bd c d e f g h i k l m n o org p parti pp press r s t tm u w x y z ", sg: " com edu gov idn net org per ", sn: " art com edu gouv org perso univ ", sy: " com edu gov mil net news org ", th: " ac co go in mi net or ", tj: " ac biz co com edu go gov info int mil name net nic org test web ", tn: " agrinet com defense edunet ens fin gov ind info intl mincom nat net org perso rnrt rns rnu tourism ",
            tz: " ac co go ne or ", ua: " biz cherkassy chernigov chernovtsy ck cn co com crimea cv dn dnepropetrovsk donetsk dp edu gov if in ivano-frankivsk kh kharkov kherson khmelnitskiy kiev kirovograd km kr ks kv lg lugansk lutsk lviv me mk net nikolaev od odessa org pl poltava pp rovno rv sebastopol sumy te ternopil uzhgorod vinnica vn zaporizhzhe zhitomir zp zt ", ug: " ac co go ne or org sc ", uk: " ac bl british-library co cym gov govt icnet jet lea ltd me mil mod national-library-scotland nel net nhs nic nls org orgn parliament plc police sch scot soc ",
            us: " dni fed isa kids nsn ", uy: " com edu gub mil net org ", ve: " co com edu gob info mil net org web ", vi: " co com k12 net org ", vn: " ac biz com edu gov health info int name net org pro ", ye: " co com gov ltd me net org plc ", yu: " ac co edu gov org ", za: " ac agric alt bourse city co cybernet db edu gov grondar iaccess imt inca landesign law mil net ngo nis nom olivetti org pix school tm web ", zm: " ac co com edu gov net org sch ", com: "ar br cn de eu gb gr hu jpn kr no qc ru sa se uk us uy za ", net: "gb jp se uk ",
            org: "ae", de: "com "
        }, has: function (r) { var d = r.lastIndexOf("."); if (0 >= d || d >= r.length - 1) return !1; var q = r.lastIndexOf(".", d - 1); if (0 >= q || q >= d - 1) return !1; var y = m.list[r.slice(d + 1)]; return y ? 0 <= y.indexOf(" " + r.slice(q + 1, d) + " ") : !1 }, is: function (r) { var d = r.lastIndexOf("."); if (0 >= d || d >= r.length - 1 || 0 <= r.lastIndexOf(".", d - 1)) return !1; var q = m.list[r.slice(d + 1)]; return q ? 0 <= q.indexOf(" " + r.slice(0, d) + " ") : !1 }, get: function (r) {
            var d = r.lastIndexOf("."); if (0 >= d || d >= r.length - 1) return null; var q = r.lastIndexOf(".", d - 1);
            if (0 >= q || q >= d - 1) return null; var y = m.list[r.slice(d + 1)]; return !y || 0 > y.indexOf(" " + r.slice(q + 1, d) + " ") ? null : r.slice(q + 1)
        }, noConflict: function () { k.SecondLevelDomains === this && (k.SecondLevelDomains = p); return this }
    }; return m
});
(function (k, p) { "object" === typeof module && module.exports ? module.exports = p(require("./punycode"), require("./IPv6"), require("./SecondLevelDomains")) : "function" === typeof define && define.amd ? define(["./punycode", "./IPv6", "./SecondLevelDomains"], p) : k.URI = p(k.punycode, k.IPv6, k.SecondLevelDomains, k) })(this, function (k, p, m, r) {
    function d(a, b) {
        var c = 1 <= arguments.length, e = 2 <= arguments.length; if (!(this instanceof d)) return c ? e ? new d(a, b) : new d(a) : new d; if (void 0 === a) {
            if (c) throw new TypeError("undefined is not a valid argument for URI");
            a = "undefined" !== typeof location ? location.href + "" : ""
        } if (null === a && c) throw new TypeError("null is not a valid argument for URI"); this.href(a); return void 0 !== b ? this.absoluteTo(b) : this
    } function q(a) { return a.replace(/([.*+?^=!:${}()|[\]\/\\])/g, "\\$1") } function y(a) { return void 0 === a ? "Undefined" : String(Object.prototype.toString.call(a)).slice(8, -1) } function x(a) { return "Array" === y(a) } function l(a, b) {
        var c = {}, e; if ("RegExp" === y(b)) c = null; else if (x(b)) { var f = 0; for (e = b.length; f < e; f++)c[b[f]] = !0 } else c[b] =
            !0; f = 0; for (e = a.length; f < e; f++)if (c && void 0 !== c[a[f]] || !c && b.test(a[f])) a.splice(f, 1), e--, f--; return a
    } function u(a, b) { var c; if (x(b)) { var e = 0; for (c = b.length; e < c; e++)if (!u(a, b[e])) return !1; return !0 } var f = y(b); e = 0; for (c = a.length; e < c; e++)if ("RegExp" === f) { if ("string" === typeof a[e] && a[e].match(b)) return !0 } else if (a[e] === b) return !0; return !1 } function F(a, b) { if (!x(a) || !x(b) || a.length !== b.length) return !1; a.sort(); b.sort(); for (var c = 0, e = a.length; c < e; c++)if (a[c] !== b[c]) return !1; return !0 } function h(a) {
        return a.replace(/^\/+|\/+$/g,
            "")
    } function t(a) { return escape(a) } function w(a) { return encodeURIComponent(a).replace(/[!'()*]/g, t).replace(/\*/g, "%2A") } function z(a) { return function (b, c) { if (void 0 === b) return this._parts[a] || ""; this._parts[a] = b || null; this.build(!c); return this } } function C(a, b) { return function (c, e) { if (void 0 === c) return this._parts[a] || ""; null !== c && (c += "", c.charAt(0) === b && (c = c.substring(1))); this._parts[a] = c; this.build(!e); return this } } var B = r && r.URI; d.version = "1.19.7"; var g = d.prototype, G = Object.prototype.hasOwnProperty;
    d._parts = function () { return { protocol: null, username: null, password: null, hostname: null, urn: null, port: null, path: null, query: null, fragment: null, preventInvalidHostname: d.preventInvalidHostname, duplicateQueryParameters: d.duplicateQueryParameters, escapeQuerySpace: d.escapeQuerySpace } }; d.preventInvalidHostname = !1; d.duplicateQueryParameters = !1; d.escapeQuerySpace = !0; d.protocol_expression = /^[a-z][a-z0-9.+-]*$/i; d.idn_expression = /[^a-z0-9\._-]/i; d.punycode_expression = /(xn--)/i; d.ip4_expression = /^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$/;
    d.ip6_expression = /^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*$/;
    d.find_uri_expression = /\b((?:[a-z][\w-]+:(?:\/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}\/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'".,<>?\u00ab\u00bb\u201c\u201d\u2018\u2019]))/ig; d.findUri = { start: /\b(?:([a-z][a-z0-9.+-]*:\/\/)|www\.)/gi, end: /[\s\r\n]|$/, trim: /[`!()\[\]{};:'".,<>?\u00ab\u00bb\u201c\u201d\u201e\u2018\u2019]+$/, parens: /(\([^\)]*\)|\[[^\]]*\]|\{[^}]*\}|<[^>]*>)/g }; d.defaultPorts = {
        http: "80", https: "443", ftp: "21",
        gopher: "70", ws: "80", wss: "443"
    }; d.hostProtocols = ["http", "https"]; d.invalid_hostname_characters = /[^a-zA-Z0-9\.\-:_]/; d.domAttributes = { a: "href", blockquote: "cite", link: "href", base: "href", script: "src", form: "action", img: "src", area: "href", iframe: "src", embed: "src", source: "src", track: "src", input: "src", audio: "src", video: "src" }; d.getDomAttribute = function (a) { if (a && a.nodeName) { var b = a.nodeName.toLowerCase(); if ("input" !== b || "image" === a.type) return d.domAttributes[b] } }; d.encode = w; d.decode = decodeURIComponent; d.iso8859 =
        function () { d.encode = escape; d.decode = unescape }; d.unicode = function () { d.encode = w; d.decode = decodeURIComponent }; d.characters = {
            pathname: { encode: { expression: /%(24|26|2B|2C|3B|3D|3A|40)/ig, map: { "%24": "$", "%26": "&", "%2B": "+", "%2C": ",", "%3B": ";", "%3D": "=", "%3A": ":", "%40": "@" } }, decode: { expression: /[\/\?#]/g, map: { "/": "%2F", "?": "%3F", "#": "%23" } } }, reserved: {
                encode: {
                    expression: /%(21|23|24|26|27|28|29|2A|2B|2C|2F|3A|3B|3D|3F|40|5B|5D)/ig, map: {
                        "%3A": ":", "%2F": "/", "%3F": "?", "%23": "#", "%5B": "[", "%5D": "]", "%40": "@",
                        "%21": "!", "%24": "$", "%26": "&", "%27": "'", "%28": "(", "%29": ")", "%2A": "*", "%2B": "+", "%2C": ",", "%3B": ";", "%3D": "="
                    }
                }
            }, urnpath: { encode: { expression: /%(21|24|27|28|29|2A|2B|2C|3B|3D|40)/ig, map: { "%21": "!", "%24": "$", "%27": "'", "%28": "(", "%29": ")", "%2A": "*", "%2B": "+", "%2C": ",", "%3B": ";", "%3D": "=", "%40": "@" } }, decode: { expression: /[\/\?#:]/g, map: { "/": "%2F", "?": "%3F", "#": "%23", ":": "%3A" } } }
        }; d.encodeQuery = function (a, b) { var c = d.encode(a + ""); void 0 === b && (b = d.escapeQuerySpace); return b ? c.replace(/%20/g, "+") : c }; d.decodeQuery =
            function (a, b) { a += ""; void 0 === b && (b = d.escapeQuerySpace); try { return d.decode(b ? a.replace(/\+/g, "%20") : a) } catch (c) { return a } }; var I = { encode: "encode", decode: "decode" }, n, A = function (a, b) { return function (c) { try { return d[b](c + "").replace(d.characters[a][b].expression, function (e) { return d.characters[a][b].map[e] }) } catch (e) { return c } } }; for (n in I) d[n + "PathSegment"] = A("pathname", I[n]), d[n + "UrnPathSegment"] = A("urnpath", I[n]); I = function (a, b, c) {
                return function (e) {
                    var f = c ? function (J) { return d[b](d[c](J)) } : d[b];
                    e = (e + "").split(a); for (var v = 0, E = e.length; v < E; v++)e[v] = f(e[v]); return e.join(a)
                }
            }; d.decodePath = I("/", "decodePathSegment"); d.decodeUrnPath = I(":", "decodeUrnPathSegment"); d.recodePath = I("/", "encodePathSegment", "decode"); d.recodeUrnPath = I(":", "encodeUrnPathSegment", "decode"); d.encodeReserved = A("reserved", "encode"); d.parse = function (a, b) {
                b || (b = { preventInvalidHostname: d.preventInvalidHostname }); var c = a.indexOf("#"); -1 < c && (b.fragment = a.substring(c + 1) || null, a = a.substring(0, c)); c = a.indexOf("?"); -1 < c && (b.query =
                    a.substring(c + 1) || null, a = a.substring(0, c)); a = a.replace(/^(https?|ftp|wss?)?:[/\\]*/, "$1://"); "//" === a.substring(0, 2) ? (b.protocol = null, a = a.substring(2), a = d.parseAuthority(a, b)) : (c = a.indexOf(":"), -1 < c && (b.protocol = a.substring(0, c) || null, b.protocol && !b.protocol.match(d.protocol_expression) ? b.protocol = void 0 : "//" === a.substring(c + 1, c + 3).replace(/\\/g, "/") ? (a = a.substring(c + 3), a = d.parseAuthority(a, b)) : (a = a.substring(c + 1), b.urn = !0))); b.path = a; return b
            }; d.parseHost = function (a, b) {
                a || (a = ""); a = a.replace(/\\/g,
                    "/"); var c = a.indexOf("/"); -1 === c && (c = a.length); if ("[" === a.charAt(0)) { var e = a.indexOf("]"); b.hostname = a.substring(1, e) || null; b.port = a.substring(e + 2, c) || null; "/" === b.port && (b.port = null) } else { var f = a.indexOf(":"); e = a.indexOf("/"); f = a.indexOf(":", f + 1); -1 !== f && (-1 === e || f < e) ? (b.hostname = a.substring(0, c) || null, b.port = null) : (e = a.substring(0, c).split(":"), b.hostname = e[0] || null, b.port = e[1] || null) } b.hostname && "/" !== a.substring(c).charAt(0) && (c++, a = "/" + a); b.preventInvalidHostname && d.ensureValidHostname(b.hostname,
                        b.protocol); b.port && d.ensureValidPort(b.port); return a.substring(c) || "/"
            }; d.parseAuthority = function (a, b) { a = d.parseUserinfo(a, b); return d.parseHost(a, b) }; d.parseUserinfo = function (a, b) { var c = a; -1 !== a.indexOf("\\") && (a = a.replace(/\\/g, "/")); var e = a.indexOf("/"), f = a.lastIndexOf("@", -1 < e ? e : a.length - 1); -1 < f && (-1 === e || f < e) ? (e = a.substring(0, f).split(":"), b.username = e[0] ? d.decode(e[0]) : null, e.shift(), b.password = e[0] ? d.decode(e.join(":")) : null, a = c.substring(f + 1)) : (b.username = null, b.password = null); return a };
    d.parseQuery = function (a, b) { if (!a) return {}; a = a.replace(/&+/g, "&").replace(/^\?*&*|&+$/g, ""); if (!a) return {}; for (var c = {}, e = a.split("&"), f = e.length, v, E, J = 0; J < f; J++)if (v = e[J].split("="), E = d.decodeQuery(v.shift(), b), v = v.length ? d.decodeQuery(v.join("="), b) : null, "__proto__" !== E) if (G.call(c, E)) { if ("string" === typeof c[E] || null === c[E]) c[E] = [c[E]]; c[E].push(v) } else c[E] = v; return c }; d.build = function (a) {
        var b = "", c = !1; a.protocol && (b += a.protocol + ":"); a.urn || !b && !a.hostname || (b += "//", c = !0); b += d.buildAuthority(a) ||
            ""; "string" === typeof a.path && ("/" !== a.path.charAt(0) && c && (b += "/"), b += a.path); "string" === typeof a.query && a.query && (b += "?" + a.query); "string" === typeof a.fragment && a.fragment && (b += "#" + a.fragment); return b
    }; d.buildHost = function (a) { var b = ""; if (a.hostname) b = d.ip6_expression.test(a.hostname) ? b + ("[" + a.hostname + "]") : b + a.hostname; else return ""; a.port && (b += ":" + a.port); return b }; d.buildAuthority = function (a) { return d.buildUserinfo(a) + d.buildHost(a) }; d.buildUserinfo = function (a) {
        var b = ""; a.username && (b += d.encode(a.username));
        a.password && (b += ":" + d.encode(a.password)); b && (b += "@"); return b
    }; d.buildQuery = function (a, b, c) { var e = "", f, v; for (f in a) if ("__proto__" !== f && G.call(a, f)) if (x(a[f])) { var E = {}; var J = 0; for (v = a[f].length; J < v; J++)void 0 !== a[f][J] && void 0 === E[a[f][J] + ""] && (e += "&" + d.buildQueryParameter(f, a[f][J], c), !0 !== b && (E[a[f][J] + ""] = !0)) } else void 0 !== a[f] && (e += "&" + d.buildQueryParameter(f, a[f], c)); return e.substring(1) }; d.buildQueryParameter = function (a, b, c) { return d.encodeQuery(a, c) + (null !== b ? "=" + d.encodeQuery(b, c) : "") };
    d.addQuery = function (a, b, c) { if ("object" === typeof b) for (var e in b) G.call(b, e) && d.addQuery(a, e, b[e]); else if ("string" === typeof b) void 0 === a[b] ? a[b] = c : ("string" === typeof a[b] && (a[b] = [a[b]]), x(c) || (c = [c]), a[b] = (a[b] || []).concat(c)); else throw new TypeError("URI.addQuery() accepts an object, string as the name parameter"); }; d.setQuery = function (a, b, c) {
        if ("object" === typeof b) for (var e in b) G.call(b, e) && d.setQuery(a, e, b[e]); else if ("string" === typeof b) a[b] = void 0 === c ? null : c; else throw new TypeError("URI.setQuery() accepts an object, string as the name parameter");
    }; d.removeQuery = function (a, b, c) {
        var e; if (x(b)) for (c = 0, e = b.length; c < e; c++)a[b[c]] = void 0; else if ("RegExp" === y(b)) for (e in a) b.test(e) && (a[e] = void 0); else if ("object" === typeof b) for (e in b) G.call(b, e) && d.removeQuery(a, e, b[e]); else if ("string" === typeof b) void 0 !== c ? "RegExp" === y(c) ? !x(a[b]) && c.test(a[b]) ? a[b] = void 0 : a[b] = l(a[b], c) : a[b] !== String(c) || x(c) && 1 !== c.length ? x(a[b]) && (a[b] = l(a[b], c)) : a[b] = void 0 : a[b] = void 0; else throw new TypeError("URI.removeQuery() accepts an object, string, RegExp as the first parameter");
    }; d.hasQuery = function (a, b, c, e) {
        switch (y(b)) { case "String": break; case "RegExp": for (var f in a) if (G.call(a, f) && b.test(f) && (void 0 === c || d.hasQuery(a, f, c))) return !0; return !1; case "Object": for (var v in b) if (G.call(b, v) && !d.hasQuery(a, v, b[v])) return !1; return !0; default: throw new TypeError("URI.hasQuery() accepts a string, regular expression or object as the name parameter"); }switch (y(c)) {
            case "Undefined": return b in a; case "Boolean": return a = !(x(a[b]) ? !a[b].length : !a[b]), c === a; case "Function": return !!c(a[b],
                b, a); case "Array": return x(a[b]) ? (e ? u : F)(a[b], c) : !1; case "RegExp": return x(a[b]) ? e ? u(a[b], c) : !1 : !(!a[b] || !a[b].match(c)); case "Number": c = String(c); case "String": return x(a[b]) ? e ? u(a[b], c) : !1 : a[b] === c; default: throw new TypeError("URI.hasQuery() accepts undefined, boolean, string, number, RegExp, Function as the value parameter");
        }
    }; d.joinPaths = function () {
        for (var a = [], b = [], c = 0, e = 0; e < arguments.length; e++) {
            var f = new d(arguments[e]); a.push(f); f = f.segment(); for (var v = 0; v < f.length; v++)"string" === typeof f[v] &&
                b.push(f[v]), f[v] && c++
        } if (!b.length || !c) return new d(""); b = (new d("")).segment(b); "" !== a[0].path() && "/" !== a[0].path().slice(0, 1) || b.path("/" + b.path()); return b.normalize()
    }; d.commonPath = function (a, b) { var c = Math.min(a.length, b.length), e; for (e = 0; e < c; e++)if (a.charAt(e) !== b.charAt(e)) { e--; break } if (1 > e) return a.charAt(0) === b.charAt(0) && "/" === a.charAt(0) ? "/" : ""; if ("/" !== a.charAt(e) || "/" !== b.charAt(e)) e = a.substring(0, e).lastIndexOf("/"); return a.substring(0, e + 1) }; d.withinString = function (a, b, c) {
        c || (c = {});
        var e = c.start || d.findUri.start, f = c.end || d.findUri.end, v = c.trim || d.findUri.trim, E = c.parens || d.findUri.parens, J = /[a-z0-9-]=["']?$/i; for (e.lastIndex = 0; ;) {
            var L = e.exec(a); if (!L) break; var P = L.index; if (c.ignoreHtml) { var N = a.slice(Math.max(P - 3, 0), P); if (N && J.test(N)) continue } var O = P + a.slice(P).search(f); N = a.slice(P, O); for (O = -1; ;) { var Q = E.exec(N); if (!Q) break; O = Math.max(O, Q.index + Q[0].length) } N = -1 < O ? N.slice(0, O) + N.slice(O).replace(v, "") : N.replace(v, ""); N.length <= L[0].length || c.ignore && c.ignore.test(N) || (O =
                P + N.length, L = b(N, P, O, a), void 0 === L ? e.lastIndex = O : (L = String(L), a = a.slice(0, P) + L + a.slice(O), e.lastIndex = P + L.length))
        } e.lastIndex = 0; return a
    }; d.ensureValidHostname = function (a, b) {
        var c = !!a, e = !1; b && (e = u(d.hostProtocols, b)); if (e && !c) throw new TypeError("Hostname cannot be empty, if protocol is " + b); if (a && a.match(d.invalid_hostname_characters)) {
            if (!k) throw new TypeError('Hostname "' + a + '" contains characters other than [A-Z0-9.-:_] and Punycode.js is not available'); if (k.toASCII(a).match(d.invalid_hostname_characters)) throw new TypeError('Hostname "' +
                a + '" contains characters other than [A-Z0-9.-:_]');
        }
    }; d.ensureValidPort = function (a) { if (a) { var b = Number(a); if (!(/^[0-9]+$/.test(b) && 0 < b && 65536 > b)) throw new TypeError('Port "' + a + '" is not a valid port'); } }; d.noConflict = function (a) {
        if (a) return a = { URI: this.noConflict() }, r.URITemplate && "function" === typeof r.URITemplate.noConflict && (a.URITemplate = r.URITemplate.noConflict()), r.IPv6 && "function" === typeof r.IPv6.noConflict && (a.IPv6 = r.IPv6.noConflict()), r.SecondLevelDomains && "function" === typeof r.SecondLevelDomains.noConflict &&
            (a.SecondLevelDomains = r.SecondLevelDomains.noConflict()), a; r.URI === this && (r.URI = B); return this
    }; g.build = function (a) { if (!0 === a) this._deferred_build = !0; else if (void 0 === a || this._deferred_build) this._string = d.build(this._parts), this._deferred_build = !1; return this }; g.clone = function () { return new d(this) }; g.valueOf = g.toString = function () { return this.build(!1)._string }; g.protocol = z("protocol"); g.username = z("username"); g.password = z("password"); g.hostname = z("hostname"); g.port = z("port"); g.query = C("query", "?");
    g.fragment = C("fragment", "#"); g.search = function (a, b) { var c = this.query(a, b); return "string" === typeof c && c.length ? "?" + c : c }; g.hash = function (a, b) { var c = this.fragment(a, b); return "string" === typeof c && c.length ? "#" + c : c }; g.pathname = function (a, b) { if (void 0 === a || !0 === a) { var c = this._parts.path || (this._parts.hostname ? "/" : ""); return a ? (this._parts.urn ? d.decodeUrnPath : d.decodePath)(c) : c } this._parts.path = this._parts.urn ? a ? d.recodeUrnPath(a) : "" : a ? d.recodePath(a) : "/"; this.build(!b); return this }; g.path = g.pathname; g.href =
        function (a, b) {
            var c; if (void 0 === a) return this.toString(); this._string = ""; this._parts = d._parts(); var e = a instanceof d, f = "object" === typeof a && (a.hostname || a.path || a.pathname); a.nodeName && (f = d.getDomAttribute(a), a = a[f] || "", f = !1); !e && f && void 0 !== a.pathname && (a = a.toString()); if ("string" === typeof a || a instanceof String) this._parts = d.parse(String(a), this._parts); else if (e || f) { e = e ? a._parts : a; for (c in e) "query" !== c && G.call(this._parts, c) && (this._parts[c] = e[c]); e.query && this.query(e.query, !1) } else throw new TypeError("invalid input");
            this.build(!b); return this
        }; g.is = function (a) {
            var b = !1, c = !1, e = !1, f = !1, v = !1, E = !1, J = !1, L = !this._parts.urn; this._parts.hostname && (L = !1, c = d.ip4_expression.test(this._parts.hostname), e = d.ip6_expression.test(this._parts.hostname), b = c || e, v = (f = !b) && m && m.has(this._parts.hostname), E = f && d.idn_expression.test(this._parts.hostname), J = f && d.punycode_expression.test(this._parts.hostname)); switch (a.toLowerCase()) {
                case "relative": return L; case "absolute": return !L; case "domain": case "name": return f; case "sld": return v;
                case "ip": return b; case "ip4": case "ipv4": case "inet4": return c; case "ip6": case "ipv6": case "inet6": return e; case "idn": return E; case "url": return !this._parts.urn; case "urn": return !!this._parts.urn; case "punycode": return J
            }return null
        }; var H = g.protocol, D = g.port, K = g.hostname; g.protocol = function (a, b) {
            if (a && (a = a.replace(/:(\/\/)?$/, ""), !a.match(d.protocol_expression))) throw new TypeError('Protocol "' + a + "\" contains characters other than [A-Z0-9.+-] or doesn't start with [A-Z]"); return H.call(this, a,
                b)
        }; g.scheme = g.protocol; g.port = function (a, b) { if (this._parts.urn) return void 0 === a ? "" : this; void 0 !== a && (0 === a && (a = null), a && (a += "", ":" === a.charAt(0) && (a = a.substring(1)), d.ensureValidPort(a))); return D.call(this, a, b) }; g.hostname = function (a, b) {
            if (this._parts.urn) return void 0 === a ? "" : this; if (void 0 !== a) {
                var c = { preventInvalidHostname: this._parts.preventInvalidHostname }; if ("/" !== d.parseHost(a, c)) throw new TypeError('Hostname "' + a + '" contains characters other than [A-Z0-9.-]'); a = c.hostname; this._parts.preventInvalidHostname &&
                    d.ensureValidHostname(a, this._parts.protocol)
            } return K.call(this, a, b)
        }; g.origin = function (a, b) { if (this._parts.urn) return void 0 === a ? "" : this; if (void 0 === a) { var c = this.protocol(); return this.authority() ? (c ? c + "://" : "") + this.authority() : "" } c = d(a); this.protocol(c.protocol()).authority(c.authority()).build(!b); return this }; g.host = function (a, b) {
            if (this._parts.urn) return void 0 === a ? "" : this; if (void 0 === a) return this._parts.hostname ? d.buildHost(this._parts) : ""; if ("/" !== d.parseHost(a, this._parts)) throw new TypeError('Hostname "' +
                a + '" contains characters other than [A-Z0-9.-]'); this.build(!b); return this
        }; g.authority = function (a, b) { if (this._parts.urn) return void 0 === a ? "" : this; if (void 0 === a) return this._parts.hostname ? d.buildAuthority(this._parts) : ""; if ("/" !== d.parseAuthority(a, this._parts)) throw new TypeError('Hostname "' + a + '" contains characters other than [A-Z0-9.-]'); this.build(!b); return this }; g.userinfo = function (a, b) {
            if (this._parts.urn) return void 0 === a ? "" : this; if (void 0 === a) {
                var c = d.buildUserinfo(this._parts); return c ?
                    c.substring(0, c.length - 1) : c
            } "@" !== a[a.length - 1] && (a += "@"); d.parseUserinfo(a, this._parts); this.build(!b); return this
        }; g.resource = function (a, b) { if (void 0 === a) return this.path() + this.search() + this.hash(); var c = d.parse(a); this._parts.path = c.path; this._parts.query = c.query; this._parts.fragment = c.fragment; this.build(!b); return this }; g.subdomain = function (a, b) {
            if (this._parts.urn) return void 0 === a ? "" : this; if (void 0 === a) {
                if (!this._parts.hostname || this.is("IP")) return ""; var c = this._parts.hostname.length - this.domain().length -
                    1; return this._parts.hostname.substring(0, c) || ""
            } c = this._parts.hostname.length - this.domain().length; c = this._parts.hostname.substring(0, c); c = new RegExp("^" + q(c)); a && "." !== a.charAt(a.length - 1) && (a += "."); if (-1 !== a.indexOf(":")) throw new TypeError("Domains cannot contain colons"); a && d.ensureValidHostname(a, this._parts.protocol); this._parts.hostname = this._parts.hostname.replace(c, a); this.build(!b); return this
        }; g.domain = function (a, b) {
            if (this._parts.urn) return void 0 === a ? "" : this; "boolean" === typeof a && (b =
                a, a = void 0); if (void 0 === a) { if (!this._parts.hostname || this.is("IP")) return ""; var c = this._parts.hostname.match(/\./g); if (c && 2 > c.length) return this._parts.hostname; c = this._parts.hostname.length - this.tld(b).length - 1; c = this._parts.hostname.lastIndexOf(".", c - 1) + 1; return this._parts.hostname.substring(c) || "" } if (!a) throw new TypeError("cannot set domain empty"); if (-1 !== a.indexOf(":")) throw new TypeError("Domains cannot contain colons"); d.ensureValidHostname(a, this._parts.protocol); !this._parts.hostname ||
                    this.is("IP") ? this._parts.hostname = a : (c = new RegExp(q(this.domain()) + "$"), this._parts.hostname = this._parts.hostname.replace(c, a)); this.build(!b); return this
        }; g.tld = function (a, b) {
            if (this._parts.urn) return void 0 === a ? "" : this; "boolean" === typeof a && (b = a, a = void 0); if (void 0 === a) { if (!this._parts.hostname || this.is("IP")) return ""; var c = this._parts.hostname.lastIndexOf("."); c = this._parts.hostname.substring(c + 1); return !0 !== b && m && m.list[c.toLowerCase()] ? m.get(this._parts.hostname) || c : c } if (a) if (a.match(/[^a-zA-Z0-9-]/)) if (m &&
                m.is(a)) c = new RegExp(q(this.tld()) + "$"), this._parts.hostname = this._parts.hostname.replace(c, a); else throw new TypeError('TLD "' + a + '" contains characters other than [A-Z0-9]'); else { if (!this._parts.hostname || this.is("IP")) throw new ReferenceError("cannot set TLD on non-domain host"); c = new RegExp(q(this.tld()) + "$"); this._parts.hostname = this._parts.hostname.replace(c, a) } else throw new TypeError("cannot set TLD empty"); this.build(!b); return this
        }; g.directory = function (a, b) {
            if (this._parts.urn) return void 0 ===
                a ? "" : this; if (void 0 === a || !0 === a) { if (!this._parts.path && !this._parts.hostname) return ""; if ("/" === this._parts.path) return "/"; var c = this._parts.path.length - this.filename().length - 1; c = this._parts.path.substring(0, c) || (this._parts.hostname ? "/" : ""); return a ? d.decodePath(c) : c } c = this._parts.path.length - this.filename().length; c = this._parts.path.substring(0, c); c = new RegExp("^" + q(c)); this.is("relative") || (a || (a = "/"), "/" !== a.charAt(0) && (a = "/" + a)); a && "/" !== a.charAt(a.length - 1) && (a += "/"); a = d.recodePath(a); this._parts.path =
                    this._parts.path.replace(c, a); this.build(!b); return this
        }; g.filename = function (a, b) {
            if (this._parts.urn) return void 0 === a ? "" : this; if ("string" !== typeof a) { if (!this._parts.path || "/" === this._parts.path) return ""; var c = this._parts.path.lastIndexOf("/"); c = this._parts.path.substring(c + 1); return a ? d.decodePathSegment(c) : c } c = !1; "/" === a.charAt(0) && (a = a.substring(1)); a.match(/\.?\//) && (c = !0); var e = new RegExp(q(this.filename()) + "$"); a = d.recodePath(a); this._parts.path = this._parts.path.replace(e, a); c ? this.normalizePath(b) :
                this.build(!b); return this
        }; g.suffix = function (a, b) {
            if (this._parts.urn) return void 0 === a ? "" : this; if (void 0 === a || !0 === a) { if (!this._parts.path || "/" === this._parts.path) return ""; var c = this.filename(), e = c.lastIndexOf("."); if (-1 === e) return ""; c = c.substring(e + 1); c = /^[a-z0-9%]+$/i.test(c) ? c : ""; return a ? d.decodePathSegment(c) : c } "." === a.charAt(0) && (a = a.substring(1)); if (c = this.suffix()) e = a ? new RegExp(q(c) + "$") : new RegExp(q("." + c) + "$"); else { if (!a) return this; this._parts.path += "." + d.recodePath(a) } e && (a = d.recodePath(a),
                this._parts.path = this._parts.path.replace(e, a)); this.build(!b); return this
        }; g.segment = function (a, b, c) {
            var e = this._parts.urn ? ":" : "/", f = this.path(), v = "/" === f.substring(0, 1); f = f.split(e); void 0 !== a && "number" !== typeof a && (c = b, b = a, a = void 0); if (void 0 !== a && "number" !== typeof a) throw Error('Bad segment "' + a + '", must be 0-based integer'); v && f.shift(); 0 > a && (a = Math.max(f.length + a, 0)); if (void 0 === b) return void 0 === a ? f : f[a]; if (null === a || void 0 === f[a]) if (x(b)) {
                f = []; a = 0; for (var E = b.length; a < E; a++)if (b[a].length ||
                    f.length && f[f.length - 1].length) f.length && !f[f.length - 1].length && f.pop(), f.push(h(b[a]))
            } else { if (b || "string" === typeof b) b = h(b), "" === f[f.length - 1] ? f[f.length - 1] = b : f.push(b) } else b ? f[a] = h(b) : f.splice(a, 1); v && f.unshift(""); return this.path(f.join(e), c)
        }; g.segmentCoded = function (a, b, c) {
            var e; "number" !== typeof a && (c = b, b = a, a = void 0); if (void 0 === b) { a = this.segment(a, b, c); if (x(a)) { var f = 0; for (e = a.length; f < e; f++)a[f] = d.decode(a[f]) } else a = void 0 !== a ? d.decode(a) : void 0; return a } if (x(b)) for (f = 0, e = b.length; f < e; f++)b[f] =
                d.encode(b[f]); else b = "string" === typeof b || b instanceof String ? d.encode(b) : b; return this.segment(a, b, c)
        }; var M = g.query; g.query = function (a, b) {
            if (!0 === a) return d.parseQuery(this._parts.query, this._parts.escapeQuerySpace); if ("function" === typeof a) { var c = d.parseQuery(this._parts.query, this._parts.escapeQuerySpace), e = a.call(this, c); this._parts.query = d.buildQuery(e || c, this._parts.duplicateQueryParameters, this._parts.escapeQuerySpace); this.build(!b); return this } return void 0 !== a && "string" !== typeof a ? (this._parts.query =
                d.buildQuery(a, this._parts.duplicateQueryParameters, this._parts.escapeQuerySpace), this.build(!b), this) : M.call(this, a, b)
        }; g.setQuery = function (a, b, c) {
            var e = d.parseQuery(this._parts.query, this._parts.escapeQuerySpace); if ("string" === typeof a || a instanceof String) e[a] = void 0 !== b ? b : null; else if ("object" === typeof a) for (var f in a) G.call(a, f) && (e[f] = a[f]); else throw new TypeError("URI.addQuery() accepts an object, string as the name parameter"); this._parts.query = d.buildQuery(e, this._parts.duplicateQueryParameters,
                this._parts.escapeQuerySpace); "string" !== typeof a && (c = b); this.build(!c); return this
        }; g.addQuery = function (a, b, c) { var e = d.parseQuery(this._parts.query, this._parts.escapeQuerySpace); d.addQuery(e, a, void 0 === b ? null : b); this._parts.query = d.buildQuery(e, this._parts.duplicateQueryParameters, this._parts.escapeQuerySpace); "string" !== typeof a && (c = b); this.build(!c); return this }; g.removeQuery = function (a, b, c) {
            var e = d.parseQuery(this._parts.query, this._parts.escapeQuerySpace); d.removeQuery(e, a, b); this._parts.query =
                d.buildQuery(e, this._parts.duplicateQueryParameters, this._parts.escapeQuerySpace); "string" !== typeof a && (c = b); this.build(!c); return this
        }; g.hasQuery = function (a, b, c) { var e = d.parseQuery(this._parts.query, this._parts.escapeQuerySpace); return d.hasQuery(e, a, b, c) }; g.setSearch = g.setQuery; g.addSearch = g.addQuery; g.removeSearch = g.removeQuery; g.hasSearch = g.hasQuery; g.normalize = function () { return this._parts.urn ? this.normalizeProtocol(!1).normalizePath(!1).normalizeQuery(!1).normalizeFragment(!1).build() : this.normalizeProtocol(!1).normalizeHostname(!1).normalizePort(!1).normalizePath(!1).normalizeQuery(!1).normalizeFragment(!1).build() };
    g.normalizeProtocol = function (a) { "string" === typeof this._parts.protocol && (this._parts.protocol = this._parts.protocol.toLowerCase(), this.build(!a)); return this }; g.normalizeHostname = function (a) { this._parts.hostname && (this.is("IDN") && k ? this._parts.hostname = k.toASCII(this._parts.hostname) : this.is("IPv6") && p && (this._parts.hostname = p.best(this._parts.hostname)), this._parts.hostname = this._parts.hostname.toLowerCase(), this.build(!a)); return this }; g.normalizePort = function (a) {
        "string" === typeof this._parts.protocol &&
        this._parts.port === d.defaultPorts[this._parts.protocol] && (this._parts.port = null, this.build(!a)); return this
    }; g.normalizePath = function (a) {
        var b = this._parts.path; if (!b) return this; if (this._parts.urn) return this._parts.path = d.recodeUrnPath(this._parts.path), this.build(!a), this; if ("/" === this._parts.path) return this; b = d.recodePath(b); var c = ""; if ("/" !== b.charAt(0)) { var e = !0; b = "/" + b } if ("/.." === b.slice(-3) || "/." === b.slice(-2)) b += "/"; b = b.replace(/(\/(\.\/)+)|(\/\.$)/g, "/").replace(/\/{2,}/g, "/"); e && (c = b.substring(1).match(/^(\.\.\/)+/) ||
            "") && (c = c[0]); for (; ;) { var f = b.search(/\/\.\.(\/|$)/); if (-1 === f) break; else if (0 === f) { b = b.substring(3); continue } var v = b.substring(0, f).lastIndexOf("/"); -1 === v && (v = f); b = b.substring(0, v) + b.substring(f + 3) } e && this.is("relative") && (b = c + b.substring(1)); this._parts.path = b; this.build(!a); return this
    }; g.normalizePathname = g.normalizePath; g.normalizeQuery = function (a) {
        "string" === typeof this._parts.query && (this._parts.query.length ? this.query(d.parseQuery(this._parts.query, this._parts.escapeQuerySpace)) : this._parts.query =
            null, this.build(!a)); return this
    }; g.normalizeFragment = function (a) { this._parts.fragment || (this._parts.fragment = null, this.build(!a)); return this }; g.normalizeSearch = g.normalizeQuery; g.normalizeHash = g.normalizeFragment; g.iso8859 = function () { var a = d.encode, b = d.decode; d.encode = escape; d.decode = decodeURIComponent; try { this.normalize() } finally { d.encode = a, d.decode = b } return this }; g.unicode = function () { var a = d.encode, b = d.decode; d.encode = w; d.decode = unescape; try { this.normalize() } finally { d.encode = a, d.decode = b } return this };
    g.readable = function () {
        var a = this.clone(); a.username("").password("").normalize(); var b = ""; a._parts.protocol && (b += a._parts.protocol + "://"); a._parts.hostname && (a.is("punycode") && k ? (b += k.toUnicode(a._parts.hostname), a._parts.port && (b += ":" + a._parts.port)) : b += a.host()); a._parts.hostname && a._parts.path && "/" !== a._parts.path.charAt(0) && (b += "/"); b += a.path(!0); if (a._parts.query) {
            for (var c = "", e = 0, f = a._parts.query.split("&"), v = f.length; e < v; e++) {
                var E = (f[e] || "").split("="); c += "&" + d.decodeQuery(E[0], this._parts.escapeQuerySpace).replace(/&/g,
                    "%26"); void 0 !== E[1] && (c += "=" + d.decodeQuery(E[1], this._parts.escapeQuerySpace).replace(/&/g, "%26"))
            } b += "?" + c.substring(1)
        } return b += d.decodeQuery(a.hash(), !0)
    }; g.absoluteTo = function (a) {
        var b = this.clone(), c = ["protocol", "username", "password", "hostname", "port"], e, f; if (this._parts.urn) throw Error("URNs do not have any generally defined hierarchical components"); a instanceof d || (a = new d(a)); if (b._parts.protocol) return b; b._parts.protocol = a._parts.protocol; if (this._parts.hostname) return b; for (e = 0; f = c[e]; e++)b._parts[f] =
            a._parts[f]; b._parts.path ? (".." === b._parts.path.substring(-2) && (b._parts.path += "/"), "/" !== b.path().charAt(0) && (c = (c = a.directory()) ? c : 0 === a.path().indexOf("/") ? "/" : "", b._parts.path = (c ? c + "/" : "") + b._parts.path, b.normalizePath())) : (b._parts.path = a._parts.path, b._parts.query || (b._parts.query = a._parts.query)); b.build(); return b
    }; g.relativeTo = function (a) {
        var b = this.clone().normalize(); if (b._parts.urn) throw Error("URNs do not have any generally defined hierarchical components"); a = (new d(a)).normalize(); var c =
            b._parts; var e = a._parts; var f = b.path(); a = a.path(); if ("/" !== f.charAt(0)) throw Error("URI is already relative"); if ("/" !== a.charAt(0)) throw Error("Cannot calculate a URI relative to another relative URI"); c.protocol === e.protocol && (c.protocol = null); if (c.username === e.username && c.password === e.password && null === c.protocol && null === c.username && null === c.password && c.hostname === e.hostname && c.port === e.port) c.hostname = null, c.port = null; else return b.build(); if (f === a) return c.path = "", b.build(); f = d.commonPath(f, a);
        if (!f) return b.build(); e = e.path.substring(f.length).replace(/[^\/]*$/, "").replace(/.*?\//g, "../"); c.path = e + c.path.substring(f.length) || "./"; return b.build()
    }; g.equals = function (a) {
        var b = this.clone(), c = new d(a); a = {}; var e; b.normalize(); c.normalize(); if (b.toString() === c.toString()) return !0; var f = b.query(); var v = c.query(); b.query(""); c.query(""); if (b.toString() !== c.toString() || f.length !== v.length) return !1; b = d.parseQuery(f, this._parts.escapeQuerySpace); v = d.parseQuery(v, this._parts.escapeQuerySpace); for (e in b) if (G.call(b,
            e)) { if (!x(b[e])) { if (b[e] !== v[e]) return !1 } else if (!F(b[e], v[e])) return !1; a[e] = !0 } for (e in v) if (G.call(v, e) && !a[e]) return !1; return !0
    }; g.preventInvalidHostname = function (a) { this._parts.preventInvalidHostname = !!a; return this }; g.duplicateQueryParameters = function (a) { this._parts.duplicateQueryParameters = !!a; return this }; g.escapeQuerySpace = function (a) { this._parts.escapeQuerySpace = !!a; return this }; return d
});
(function (k, p) { "object" === typeof module && module.exports ? module.exports = p(require("./URI")) : "function" === typeof define && define.amd ? define(["./URI"], p) : k.URITemplate = p(k.URI, k) })(this, function (k, p) {
    function m(l) { if (m._cache[l]) return m._cache[l]; if (!(this instanceof m)) return new m(l); this.expression = l; m._cache[l] = this; return this } function r(l) { this.data = l; this.cache = {} } var d = p && p.URITemplate, q = Object.prototype.hasOwnProperty, y = m.prototype, x = {
        "": {
            prefix: "", separator: ",", named: !1, empty_name_separator: !1,
            encode: "encode"
        }, "+": { prefix: "", separator: ",", named: !1, empty_name_separator: !1, encode: "encodeReserved" }, "#": { prefix: "#", separator: ",", named: !1, empty_name_separator: !1, encode: "encodeReserved" }, ".": { prefix: ".", separator: ".", named: !1, empty_name_separator: !1, encode: "encode" }, "/": { prefix: "/", separator: "/", named: !1, empty_name_separator: !1, encode: "encode" }, ";": { prefix: ";", separator: ";", named: !0, empty_name_separator: !1, encode: "encode" }, "?": { prefix: "?", separator: "&", named: !0, empty_name_separator: !0, encode: "encode" },
        "&": { prefix: "&", separator: "&", named: !0, empty_name_separator: !0, encode: "encode" }
    }; m._cache = {}; m.EXPRESSION_PATTERN = /\{([^a-zA-Z0-9%_]?)([^\}]+)(\}|$)/g; m.VARIABLE_PATTERN = /^([^*:.](?:\.?[^*:.])*)((\*)|:(\d+))?$/; m.VARIABLE_NAME_PATTERN = /[^a-zA-Z0-9%_.]/; m.LITERAL_PATTERN = /[<>{}"`^| \\]/; m.expand = function (l, u, F) {
        var h = x[l.operator], t = h.named ? "Named" : "Unnamed"; l = l.variables; var w = [], z, C; for (C = 0; z = l[C]; C++) {
            var B = u.get(z.name); if (0 === B.type && F && F.strict) throw Error('Missing expansion value for variable "' +
                z.name + '"'); if (B.val.length) { if (1 < B.type && z.maxlength) throw Error('Invalid expression: Prefix modifier not applicable to variable "' + z.name + '"'); w.push(m["expand" + t](B, h, z.explode, z.explode && h.separator || ",", z.maxlength, z.name)) } else B.type && w.push("")
        } return w.length ? h.prefix + w.join(h.separator) : ""
    }; m.expandNamed = function (l, u, F, h, t, w) {
        var z = "", C = u.encode; u = u.empty_name_separator; var B = !l[C].length, g = 2 === l.type ? "" : k[C](w), G; var I = 0; for (G = l.val.length; I < G; I++) {
            if (t) {
                var n = k[C](l.val[I][1].substring(0,
                    t)); 2 === l.type && (g = k[C](l.val[I][0].substring(0, t)))
            } else B ? (n = k[C](l.val[I][1]), 2 === l.type ? (g = k[C](l.val[I][0]), l[C].push([g, n])) : l[C].push([void 0, n])) : (n = l[C][I][1], 2 === l.type && (g = l[C][I][0])); z && (z += h); F ? z += g + (u || n ? "=" : "") + n : (I || (z += k[C](w) + (u || n ? "=" : "")), 2 === l.type && (z += g + ","), z += n)
        } return z
    }; m.expandUnnamed = function (l, u, F, h, t) {
        var w = "", z = u.encode; u = u.empty_name_separator; var C = !l[z].length, B; var g = 0; for (B = l.val.length; g < B; g++) {
            if (t) var G = k[z](l.val[g][1].substring(0, t)); else C ? (G = k[z](l.val[g][1]),
                l[z].push([2 === l.type ? k[z](l.val[g][0]) : void 0, G])) : G = l[z][g][1]; w && (w += h); if (2 === l.type) { var I = t ? k[z](l.val[g][0].substring(0, t)) : l[z][g][0]; w += I; w = F ? w + (u || G ? "=" : "") : w + "," } w += G
        } return w
    }; m.noConflict = function () { p.URITemplate === m && (p.URITemplate = d); return m }; y.expand = function (l, u) { var F = ""; this.parts && this.parts.length || this.parse(); l instanceof r || (l = new r(l)); for (var h = 0, t = this.parts.length; h < t; h++)F += "string" === typeof this.parts[h] ? this.parts[h] : m.expand(this.parts[h], l, u); return F }; y.parse = function () {
        var l =
            this.expression, u = m.EXPRESSION_PATTERN, F = m.VARIABLE_PATTERN, h = m.VARIABLE_NAME_PATTERN, t = m.LITERAL_PATTERN, w = [], z = 0, C = function (A) { if (A.match(t)) throw Error('Invalid Literal "' + A + '"'); return A }; for (u.lastIndex = 0; ;) {
                var B = u.exec(l); if (null === B) { w.push(C(l.substring(z))); break } else w.push(C(l.substring(z, B.index))), z = B.index + B[0].length; if (!x[B[1]]) throw Error('Unknown Operator "' + B[1] + '" in "' + B[0] + '"'); if (!B[3]) throw Error('Unclosed Expression "' + B[0] + '"'); var g = B[2].split(","); for (var G = 0, I = g.length; G <
                    I; G++) { var n = g[G].match(F); if (null === n) throw Error('Invalid Variable "' + g[G] + '" in "' + B[0] + '"'); if (n[1].match(h)) throw Error('Invalid Variable Name "' + n[1] + '" in "' + B[0] + '"'); g[G] = { name: n[1], explode: !!n[3], maxlength: n[4] && parseInt(n[4], 10) } } if (!g.length) throw Error('Expression Missing Variable(s) "' + B[0] + '"'); w.push({ expression: B[0], operator: B[1], variables: g })
            } w.length || w.push(C(l)); this.parts = w; return this
    }; r.prototype.get = function (l) {
        var u = this.data, F = { type: 0, val: [], encode: [], encodeReserved: [] };
        if (void 0 !== this.cache[l]) return this.cache[l]; this.cache[l] = F; u = "[object Function]" === String(Object.prototype.toString.call(u)) ? u(l) : "[object Function]" === String(Object.prototype.toString.call(u[l])) ? u[l](l) : u[l]; if (void 0 !== u && null !== u) if ("[object Array]" === String(Object.prototype.toString.call(u))) { var h = 0; for (l = u.length; h < l; h++)void 0 !== u[h] && null !== u[h] && F.val.push([void 0, String(u[h])]); F.val.length && (F.type = 3) } else if ("[object Object]" === String(Object.prototype.toString.call(u))) {
            for (h in u) q.call(u,
                h) && void 0 !== u[h] && null !== u[h] && F.val.push([h, String(u[h])]); F.val.length && (F.type = 2)
        } else F.type = 1, F.val.push([void 0, String(u)]); return F
    }; k.expand = function (l, u) { var F = (new m(l)).expand(u); return new k(F) }; return m
});
(function (k, p) { "object" === typeof module && module.exports ? module.exports = p(require("jquery"), require("./URI")) : "function" === typeof define && define.amd ? define(["jquery", "./URI"], p) : p(k.jQuery, k.URI) })(this, function (k, p) {
    function m(h) { return h.replace(/([.*+?^=!:${}()|[\]\/\\])/g, "\\$1") } function r(h) { var t = h.nodeName.toLowerCase(); if ("input" !== t || "image" === h.type) return p.domAttributes[t] } function d(h) { return { get: function (t) { return k(t).uri()[h]() }, set: function (t, w) { k(t).uri()[h](w); return w } } } function q(h,
        t) { if (!r(h) || !t) return !1; var w = t.match(u); if (!w || !w[5] && ":" !== w[2] && !x[w[2]]) return !1; var z = k(h).uri(); if (w[5]) return z.is(w[5]); if (":" === w[2]) { var C = w[1].toLowerCase() + ":"; return x[C] ? x[C](z, w[4]) : !1 } C = w[1].toLowerCase(); return y[C] ? x[w[2]](z[C](), w[4], C) : !1 } var y = {}, x = {
            "=": function (h, t) { return h === t }, "^=": function (h, t) { return !!(h + "").match(new RegExp("^" + m(t), "i")) }, "$=": function (h, t) { return !!(h + "").match(new RegExp(m(t) + "$", "i")) }, "*=": function (h, t, w) {
                "directory" === w && (h += "/"); return !!(h + "").match(new RegExp(m(t),
                    "i"))
            }, "equals:": function (h, t) { return h.equals(t) }, "is:": function (h, t) { return h.is(t) }
        }; k.each("origin authority directory domain filename fragment hash host hostname href password path pathname port protocol query resource scheme search subdomain suffix tld username".split(" "), function (h, t) { y[t] = !0; k.attrHooks["uri:" + t] = d(t) }); var l = function (h, t) { return k(h).uri().href(t).toString() }; k.each(["src", "href", "action", "uri", "cite"], function (h, t) { k.attrHooks[t] = { set: l } }); k.attrHooks.uri.get = function (h) { return k(h).uri() };
    k.fn.uri = function (h) { var t = this.first(), w = t.get(0), z = r(w); if (!z) throw Error('Element "' + w.nodeName + '" does not have either property: href, src, action, cite'); if (void 0 !== h) { var C = t.data("uri"); if (C) return C.href(h); h instanceof p || (h = p(h || "")) } else { if (h = t.data("uri")) return h; h = p(t.attr(z) || "") } h._dom_element = w; h._dom_attribute = z; h.normalize(); t.data("uri", h); return h }; p.prototype.build = function (h) {
        if (this._dom_element) this._string = p.build(this._parts), this._deferred_build = !1, this._dom_element.setAttribute(this._dom_attribute,
            this._string), this._dom_element[this._dom_attribute] = this._string; else if (!0 === h) this._deferred_build = !0; else if (void 0 === h || this._deferred_build) this._string = p.build(this._parts), this._deferred_build = !1; return this
    }; var u = /^([a-zA-Z]+)\s*([\^\$*]?=|:)\s*(['"]?)(.+)\3|^\s*([a-zA-Z0-9]+)\s*$/; var F = k.expr.createPseudo ? k.expr.createPseudo(function (h) { return function (t) { return q(t, h) } }) : function (h, t, w) { return q(h, w[3]) }; k.expr[":"].uri = F; return k
});
(function (k, p) { "object" === typeof module && module.exports ? module.exports = p(require("./URI")) : "function" === typeof define && define.amd ? define(["./URI"], p) : p(k.URI) })(this, function (k) {
    var p = k.prototype, m = p.fragment; k.fragmentPrefix = "?"; var r = k._parts; k._parts = function () { var d = r(); d.fragmentPrefix = k.fragmentPrefix; return d }; p.fragmentPrefix = function (d) { this._parts.fragmentPrefix = d; return this }; p.fragment = function (d, q) {
        var y = this._parts.fragmentPrefix, x = this._parts.fragment || ""; return !0 === d ? x.substring(0,
            y.length) !== y ? {} : k.parseQuery(x.substring(y.length)) : void 0 !== d && "string" !== typeof d ? (this._parts.fragment = y + k.buildQuery(d), this.build(!q), this) : m.call(this, d, q)
    }; p.addFragment = function (d, q, y) { var x = this._parts.fragmentPrefix, l = k.parseQuery((this._parts.fragment || "").substring(x.length)); k.addQuery(l, d, q); this._parts.fragment = x + k.buildQuery(l); "string" !== typeof d && (y = q); this.build(!y); return this }; p.removeFragment = function (d, q, y) {
        var x = this._parts.fragmentPrefix, l = k.parseQuery((this._parts.fragment ||
            "").substring(x.length)); k.removeQuery(l, d, q); this._parts.fragment = x + k.buildQuery(l); "string" !== typeof d && (y = q); this.build(!y); return this
    }; p.setFragment = function (d, q, y) { var x = this._parts.fragmentPrefix, l = k.parseQuery((this._parts.fragment || "").substring(x.length)); k.setQuery(l, d, q); this._parts.fragment = x + k.buildQuery(l); "string" !== typeof d && (y = q); this.build(!y); return this }; p.addHash = p.addFragment; p.removeHash = p.removeFragment; p.setHash = p.setFragment; return k
});
