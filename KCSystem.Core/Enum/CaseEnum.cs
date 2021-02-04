using System;
using System.Collections.Generic;
using System.Text;

namespace KCSystem.Core.Enum
{
    /// <summary>
    /// 联系人身份
    /// </summary>
    public enum ContactPersonEnum
    {
        本人 = 0,
        配偶 = 1,
        父亲 = 2,
        母亲 = 3,
        子女 = 4,
        担保人 = 5,
        其他 = 6
    }

    /// <summary>
    /// 企查查公司注册状态
    /// </summary>
    public enum CompanyStatusEnum
    {
        存续 = 0,
        在业 = 1,
        吊销 = 2,
        注销 = 3,
        迁入 = 4,
        迁出 = 5,
        停业 = 6,
        清算 = 7
    }
    /// <summary>
    /// 执行回款状态
    /// </summary>
    public enum ExecutionType
    {
        [SelectDisplayName("已冻结")]
        已冻结 = 0,
        [SelectDisplayName("执行划扣")]
        执行划扣 = 1,
        [SelectDisplayName("银行划扣")]
        银行划扣 = 2,
        
    }

    /// <summary>
    /// 还款状态 ，，
    /// </summary>
    public enum RepaymentStateEnum
    {
        [SelectDisplayName("已还款")]
        已还款 = 1,
        [SelectDisplayName("未还款")]
        未还款 = 2,
        [SelectDisplayName("过期未还")]
        过期未还 = 3,
        [SelectDisplayName("未结清")]
        未结清 = 4
    }

    /// <summary>
    /// 诉催一体
    /// </summary>
    public enum UrgEnum
    {
        [SelectDisplayName("诉催一体")]
        诉催一体 = 1,
        [SelectDisplayName("单委")]
        单委 = 2,
        [SelectDisplayName("其他")]
        其他 = 3
    }


    /// <summary>
    /// 清收方向
    /// </summary>
    public enum DirectionEnum
    {
        [SelectDisplayName("结清")]
        结清 = 1,
        [SelectDisplayName("分期")]
        分期 = 2,
        [SelectDisplayName("清逾")]
        清逾 = 3,
        [SelectDisplayName("压降")]
        压降 = 4,
        [SelectDisplayName("沟通")]
        沟通 = 5,
        [SelectDisplayName("退案")]
        退案 = 6
    }

    /// <summary>
    /// 还款类型
    /// </summary>
    public enum RepaymentTypeEnum
    {
        [SelectDisplayName("结清")]
        结清 = 1,
        [SelectDisplayName("分期")]
        分期 = 2,
        [SelectDisplayName("清逾")]
        清逾 = 3,
        [SelectDisplayName("压降")]
        压降 = 4,
        [SelectDisplayName("部分还款")]
        部分还款 = 5
    }

    /// <summary>
    /// 批量修改数据类型
    /// </summary>
    public enum UpdateFiledEnum
    {
        [SelectDisplayName("诉催一体")]
        诉催一体 = 1,
        [SelectDisplayName("律所")]
        律所 = 2,
        [SelectDisplayName("案件状态")]
        案件状态 = 3
    }


    /// <summary>
    /// 查封状态（预查封，已查封）
    /// </summary>
    public enum SeizureStateEnum
    {
        [SelectDisplayName("预查封")]
        预查封 = 1,
        [SelectDisplayName("已查封")]
        已查封 = 2
    }

    /// <summary>
    /// 案件状态
    /// </summary>
    public enum CaseEndEnum
    {
        [SelectDisplayName("正常")]
        正常 = 0,
        [SelectDisplayName("已退案")]
        已退案 = 1
    }

    /// <summary>
    /// 案件状态
    /// </summary>
    public enum OtherCaseEndEnum
    {
        [SelectDisplayName("正常")]
        正常 = 0
    }

    /// <summary>
    /// 电话状态
    /// </summary>
    public enum PhoneStateEnum
    {
        [SelectDisplayName("关机")]
        关机 = 0,
        [SelectDisplayName("空号")]
        空号 = 1,
        [SelectDisplayName("可联")]
        可联 = 2,
        [SelectDisplayName("未接")]
        未接 = 3,
        [SelectDisplayName("设置")]
        设置 = 4,
        [SelectDisplayName("停机")]
        停机 = 5,
        [SelectDisplayName("无效")]
        无效 = 6,
        [SelectDisplayName("其他")]
        其他 =7
    }

    /// <summary>
    /// 电催类型
    /// </summary>
    public enum RecoveryPhoneTypeEnum
    {
        [SelectDisplayName("电话")]
        电话 = 1,
        [SelectDisplayName("短信")]
        短信 = 2
    }

    /// <summary>
    /// 号码类型
    /// </summary>
    public enum PhoneTypeEnum
    {
        [SelectDisplayName("号码1")]
        号码1 = 1,
        [SelectDisplayName("号码2")]
        号码2 = 2,
        [SelectDisplayName("号码3")]
        号码3 = 3
    }

    /// <summary>
    /// 短信类型 
    /// </summary>
    public enum MessageTypeEnum
    {
        [SelectDisplayName("正常沟通")]
        正常沟通 = 1,
        [SelectDisplayName("司法文书")]
        司法文书 = 2
    }

    /// <summary>
    /// 地址类型
    /// </summary>
    public enum AddressTypeEnum
    {
        [SelectDisplayName("户籍地")]
        户籍地 = 1,
        [SelectDisplayName("查封地址")]
        查封地址 = 2,
        [SelectDisplayName("有效地址")]
        有效地址 = 3,
        [SelectDisplayName("面谈")]
        面谈 = 4
    }

    /// <summary>
    /// 邮寄地址类型
    /// </summary>
    public enum MailAddressEnum
    {
        身份证地址 = 0,
        合同中送达条款 = 1,
        送达地址确认书地址 = 2,
        申请表地址 = 3,
        借款合同主体信息地址 = 4,
        担保合同主体信息地址 = 5
    }

    /// <summary>
    /// 电话状态
    /// </summary>
    public enum TomorrowTimeEnum
    {
        [SelectDisplayName("上午")]
        上午 = 1,
        [SelectDisplayName("下午")]
        下午 = 2
    }

    /// <summary>
    /// 担保方式
    /// </summary>
    public enum GuaranteeModeEnum
    {
        保证担保 = 0,
        联保 = 1
    }

    /// <summary>
    /// 担保方式
    /// </summary>
    public enum GuaranteeClassesEnum
    {
        担保函 = 0,
        担保合同 = 1
    }

    /// <summary>
    /// 担保方式
    /// </summary>
    public enum GuaranteeNameEnum
    {
        担保函 = 0,
        联合体担保合同 = 1,
        保证担保合同 = 2
    }

    /// <summary>
    /// 抵押类型
    /// </summary>
    public enum MortgageTypeEnum
    {
        房产 = 0,
        车辆 = 1
    }

    /// <summary>
    /// 公告类型
    /// </summary>
    public enum NoticeTypeEnum
    {
        [SelectDisplayName("立案公告")]
        立案公告 = 0,
        [SelectDisplayName("判决公告")]
        判决公告 = 1
    }

    /// <summary>
    /// 庭审结果
    /// </summary>
    public enum TrialResultEnum
    {
        [SelectDisplayName("正常")]
        正常 = 0,
        [SelectDisplayName("异常")]
        异常 = 1
    }

    public enum SexEnum
    {
        [SelectDisplayName("男")]
        男 = 0,
        [SelectDisplayName("女")]
        女 = 1,
        [SelectDisplayName("未知")]
        未知 = 2
    }

    /// <summary>
    /// 判决结果
    /// </summary>
    public enum JudgmentResultEnum
    {
        [SelectDisplayName("胜诉")]
        胜诉 = 0,
        [SelectDisplayName("败诉")]
        败诉 = 1,
        [SelectDisplayName("部分败诉")]
        部分败诉 = 2
    }

    /// <summary>
    /// 付款方式
    /// </summary>
    public enum PayTypeEnum
    {
        [SelectDisplayName("公对公")]
        公对公 = 0,
        [SelectDisplayName("私对私")]
        私对私 = 1
    }

    /// <summary>
    /// 查封类型
    /// </summary>
    public enum SealedTypeEnum
    {
        [SelectDisplayName("查封")]
        查封 = 0,
        [SelectDisplayName("续封")]
        续封 = 1
    }

    /// <summary>
    /// 第几轮查封
    /// </summary>
    public enum SealedNoEnum
    {
        [SelectDisplayName("第一轮")]
        第一轮 = 1,
        [SelectDisplayName("第二轮")]
        第二轮 = 2,
        [SelectDisplayName("第三轮")]
        第三轮 = 3,
        [SelectDisplayName("第四轮")]
        第四轮 = 4,
        [SelectDisplayName("第五轮")]
        第五轮 = 5
    }

    /// <summary>
    /// 诉讼进展
    /// </summary>
    public enum CaseProgressEnum
    {
        [SelectDisplayName("一审")]
        一审 = 1,
        [SelectDisplayName("二审")]
        二审 = 2,
        [SelectDisplayName("再审")]
        再审 = 3,
        [SelectDisplayName("执行")]
        执行 = 4       
    }

    #region 清收客户分析表
    /// <summary>
    /// 客户分类
    /// </summary>
    public enum CustomerTypeEnum
    {
        [SelectDisplayName("A、法院已调解固化。")]
        A = 1,
        [SelectDisplayName("B、已确定方案，已准备资料待批复、或在准备资料中。")]
        B = 2,
        [SelectDisplayName("C、可联系但不能形成还款方案，有意向现达不到调解要求。")]
        C = 3,
        [SelectDisplayName("D、电话通长期未接、关机、设置")]
        D = 4,
        [SelectDisplayName("E、空号、停机")]
        E = 5
    }

    /// <summary>
    /// 逾期情况_逾期金额
    /// </summary>
    public enum OverdueAmountEnum
    {
        [SelectDisplayName("10万以下")]
        十万以下 = 1,
        [SelectDisplayName("10-30万")]
        十万至三十万 = 2,
        [SelectDisplayName("30-50万")]
        三十万至五十万 = 3,
        [SelectDisplayName("50万以上")]
        五十万以上 = 4
    }

    /// <summary>
    /// 逾期情况_逾期天数
    /// </summary>
    public enum OverdueDayCountEnum
    {
        [SelectDisplayName("90天以内")]
        九十天以内 = 1,
        [SelectDisplayName("90-200天")]
        九十至两百天 = 2,
        [SelectDisplayName("200-360天")]
        两百至三百六十天 = 3,
        [SelectDisplayName("360-720天")]
        三百六十至七百二十天 = 4,
        [SelectDisplayName("720-1080天")]
        七百二十至一千零八十天 = 5,
        [SelectDisplayName("1080天以上")]
        一千零八十天以上 = 6
    }

    /// <summary>
    /// 可联情况_可联情况
    /// </summary>
    public enum ConnectableSituationEnum
    {
        [SelectDisplayName("正常接听")]
        正常接听 = 1,
        [SelectDisplayName("通了不接")]
        通了不接 = 2,
        [SelectDisplayName("设置关机")]
        设置关机 = 3,
        [SelectDisplayName("空号停机")]
        空号停机 = 4
    }


    /// <summary>
    /// 可联情况_是否加微信
    /// </summary>
    public enum IsAddWechatEnum
    {
        [SelectDisplayName("已加")]
        已加 = 1,
        [SelectDisplayName("未加")]
        未加 = 2,
        [SelectDisplayName("无效")]
        无效 =3
    }

    /// <summary>
    /// 可联情况_直系亲属可联情况
    /// </summary>
    public enum ImmediateFamilyConnectableEnum
    {
        [SelectDisplayName("无电话")]
        无电话 = 1,
        [SelectDisplayName("正常接听")]
        正常接听 = 2,
        [SelectDisplayName("通了不接")]
        通了不接 = 3,
        [SelectDisplayName("设置关机")]
        设置关机 = 4,
        [SelectDisplayName("空号停机")]
        空号停机 = 5
    }

    /// <summary>
    /// 借款人基本信息_年龄
    /// </summary>
    public enum BorrowerAgeEnum
    {
        [SelectDisplayName("30岁以下")]
        三十岁以下 = 1,
        [SelectDisplayName("30-40")]
        三十至四十岁 = 2,
        [SelectDisplayName("40-50")]
        四十至五十岁 = 3,
        [SelectDisplayName("50-60")]
        五十至六十岁 = 4,
        [SelectDisplayName("60以上")]
        六十岁以上 = 5
    }

    /// <summary>
    /// 借款人基本信息_户口
    /// </summary>
    public enum BorrowerCensusEnum
    {
        [SelectDisplayName("本地")]
        本地 = 1,
        [SelectDisplayName("省内他市")]
        省内他市 = 2,
        [SelectDisplayName("外省")]
        外省 = 3
    }

    /// <summary>
    /// 借款人基本信息_身份证
    /// </summary>
    public enum BorrowerIdCardEnum
    {
        [SelectDisplayName("本地")]
        本地 = 1,
        [SelectDisplayName("省内他市")]
        省内他市 = 2,
        [SelectDisplayName("外省")]
        外省 = 3
    }

    /// <summary>
    /// 借款人基本信息_婚姻状况
    /// </summary>
    public enum BorrowerMaritalEnum
    {
        [SelectDisplayName("未婚单身")]
        未婚单身 = 1,
        [SelectDisplayName("已婚")]
        已婚 = 2,
        [SelectDisplayName("离异")]
        离异 = 3,
        [SelectDisplayName("丧偶")]
        丧偶 = 4,
        [SelectDisplayName("不确定")]
        不确定 = 5
    }

    /// <summary>
    /// 平安诉讼及保全_平安诉讼状态
    /// </summary>
    public enum LitigationStatesEnum
    {
        [SelectDisplayName("未诉")]
        未诉 = 1,
        [SelectDisplayName("已诉未判")]
        已诉未判 = 2,
        [SelectDisplayName("已判决")]
        已判决 = 3,
        [SelectDisplayName("已执行")]
        已执行 = 4,
        [SelectDisplayName("调解固化")]
        调解固化 = 5,
        [SelectDisplayName("不确定")]
        不确定 = 6
    }

    public enum RecoveryLitigationStatesEnum
    {
        [SelectDisplayName("未诉")]
        未诉 = 1,
        [SelectDisplayName("立案未开庭")]
        立案未开庭 = 2,
        [SelectDisplayName("开庭未判决")]
        开庭未判决 = 3,
        [SelectDisplayName("已判决")]
        已判决 = 4,
        [SelectDisplayName("执行中")]
        执行中 = 5,
        [SelectDisplayName("终本")]
        终本 = 6,
        [SelectDisplayName("不确定")]
        不确定 = 7
    }

    public enum SecondaryActionEnum
    {
        [SelectDisplayName("开庭")]
        开庭 = 1,
        [SelectDisplayName("执行")]
        执行 = 2,
        [SelectDisplayName("评估")]
        评估 = 3,
        [SelectDisplayName("上拍")]
        上拍 = 4
    }

    /// <summary>
    /// 产品类型
    /// </summary>
    public enum ProductTypeEnum
    {
        [SelectDisplayName("住房抵押")]
        住房抵押 = 1,
        [SelectDisplayName("其他抵押")]
        其他抵押 = 2,
        [SelectDisplayName("信用（含10万以内）")]
        信用_含10万以内 = 3,
        [SelectDisplayName("信用（10万以上）")]
        信用_10万以上 = 4,
        [SelectDisplayName("保证")]
        保证 = 5
    }
    
    /// <summary>
    /// 平安诉讼及保全_保全情况
    /// </summary>
    public enum PreservationEnum
    {
        [SelectDisplayName("无")]
        无 = 1,
        [SelectDisplayName("房产保全")]
        房产保全 = 2,
        [SelectDisplayName("账户保全")]
        账户保全 = 3,
        [SelectDisplayName("不确定")]
        不确定 = 4
    }

    /// <summary>
    /// 其他被执和诉讼_其他被执行
    /// </summary>
    public enum OtherLitigationEnum
    {
        [SelectDisplayName("无")]
        无 = 1,
        [SelectDisplayName("1笔")]
        一笔 = 2,
        [SelectDisplayName("2笔")]
        两笔 = 3,
        [SelectDisplayName("3笔")]
        三笔 = 4,
        [SelectDisplayName("3笔以上")]
        三笔以上 = 5
    }

    /// <summary>
    /// 其他重要信息_其他负债情况
    /// </summary>
    public enum OtherLiabilitiesEnum
    {
        [SelectDisplayName("无")]
        无 = 1,
        [SelectDisplayName("10万以内")]
        十万以内 = 2,
        [SelectDisplayName("10-30万")]
        十万至三十万 = 3,
        [SelectDisplayName("30-60万")]
        三十万至六十万 = 4,
        [SelectDisplayName("60-100万")]
        六十万至一百万 = 5,
        [SelectDisplayName("100-200万")]
        一百万至两百万 = 6,
        [SelectDisplayName("200万以上")]
        两百万以上 = 7,
        [SelectDisplayName("不确定")]
        不确定 = 8
    }

    /// <summary>
    /// 其他重要信息_有无重组救济
    /// </summary>
    public enum ReliefEnum
    {
        [SelectDisplayName("无")]
        无 = 1,
        [SelectDisplayName("有")]
        有 = 2,
        [SelectDisplayName("不确定")]
        不确定 = 3
    }

    /// <summary>
    /// 收入情况_目前是否在武汉
    /// </summary>
    public enum OldPlaceEnum
    {
        [SelectDisplayName("是")]
        是 = 1,
        [SelectDisplayName("否")]
        否 = 2,
        [SelectDisplayName("不确定")]
        不确定 = 3,
        [SelectDisplayName("不用")]
        不用 = 4
    }

    /// <summary>
    /// 收入情况_目前是否在武汉
    /// </summary>
    public enum ThreeNumberEnum
    {
        [SelectDisplayName("是")]
        是 = 1,
        [SelectDisplayName("否")]
        否 = 2,
        [SelectDisplayName("不确定")]
        不确定 = 3 ,
        [SelectDisplayName("不用")]
        不用 = 4
    }

    /// <summary>
    /// 收入情况_家庭月收入状况
    /// </summary>
    public enum FamilyMontherMoneyEnum
    {
        [SelectDisplayName("5000以内")]
        五千以内 = 1,
        [SelectDisplayName("5000-1万")]
        五千至一万 = 2,
        [SelectDisplayName("1万-2万")]
        一万至两万 = 3,
        [SelectDisplayName("2万以上")]
        两万以上 = 4,
        [SelectDisplayName("不确定")]
        不确定 = 5
    }

    /// <summary>
    /// 电话联系情况 短信彩信情况	
    /// </summary>
    public enum ThreeCaseEnum
    {
        [SelectDisplayName("是")]
        是 = 1,
        [SelectDisplayName("否")]
        否 = 2,
        [SelectDisplayName("不用")]
        不用 = 3
    }

    /// <summary>
    /// 外访情况 
    /// </summary>
    public enum ExternalVisitsEnum
    {
        [SelectDisplayName("是")]
        是 = 1,
        [SelectDisplayName("否")]
        否 = 2,
        [SelectDisplayName("无地址")]
        无地址 = 3,
        [SelectDisplayName("不用")]
        不用 = 4
    }

    /// <summary>
    /// 面谈情况 
    /// </summary>
    public enum InterviewEnum
    {
        [SelectDisplayName("未面谈")]
        未面谈 = 1,
        [SelectDisplayName("面谈一次")]
        面谈一次 = 2,
        [SelectDisplayName("面谈两次")]
        面谈两次 = 3,
        [SelectDisplayName("两次以上")]
        两次以上 = 4
    }

    /// <summary>
    /// 还款意愿  
    /// </summary>
    public enum WillingnessToRepayEnum
    {
        [SelectDisplayName("有意愿")]
        有意愿 = 1,
        [SelectDisplayName("无意愿")]
        无意愿 = 2,
        [SelectDisplayName("未找到人")]
        未找到人 = 3
    }

    /// <summary>
    /// 还款方案确定  
    /// </summary>
    public enum RepaymentPlanEnum
    {
        [SelectDisplayName("已确定")]
        已确定 = 1,
        [SelectDisplayName("未确定")]
        未确定 = 2,
        [SelectDisplayName("未找到人")]
        未找到人 = 3
    }

    /// <summary>
    /// 还款情况  
    /// </summary>
    public enum RepaymentEnum
    {
        [SelectDisplayName("无")]
        无 = 1,
        [SelectDisplayName("5000以内")]
        五千以内 = 2,
        [SelectDisplayName("5000-1万")]
        五千至一万 = 3,
        [SelectDisplayName("1万-5万")]
        一万至五万 = 4,
        [SelectDisplayName("5万以上")]
        五万以上 = 5
    }
    #endregion


    #region 法务KPI
    /// <summary>
    /// 法务KPI流程
    /// </summary>
    public enum LegalKpiProcessEnum
    {
        诉前=0,
        审判,
        执行,
        调解
    }

    public enum LegalKpiRoleEnum
    {
        项目经理=0,
        法务专员内务,
        法务专员派驻
    }
    #endregion

    /// <summary>
    /// 自定义注解属性
    /// </summary>
    public class SelectDisplayNameAttribute : Attribute
    {
        private string _diaplayName;
        public string DisplayName
        {
            get { return _diaplayName; }
        }

        public SelectDisplayNameAttribute(string displayName)
        {
            _diaplayName = displayName;
        }
    }

    /// <summary>
    /// 审判文书类型
    /// </summary>
    public enum TrialDocType
    {
        判决书=0,
        调解书,
        撤诉裁定书,
    }
    /// <summary>
    /// 执行具体进展
    /// </summary>
    public enum ConcreteProgressImplementation{
        首封拍卖=0,
        轮候参与分配

    }

    /// <summary>
    /// 审判文书类型
    /// </summary>
    public enum ChangeTypeEnum
    {
        向前变更 = 1,
        往后变更 = 2
    }
}
