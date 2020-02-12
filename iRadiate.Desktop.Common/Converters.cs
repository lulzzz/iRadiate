using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;

using System.Windows;

using MahApps.Metro.IconPacks;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common.Diary;


namespace iRadiate.Desktop.Common
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class OverlapToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return 0.5;
            }
            else
            {
                return 1;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class IntAdditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return ((Int32)value) + (Int32)parameter;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleAdditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return ((Double)value) + (Int32)parameter;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WorkflowStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TaskStatus))
            {
                return parameter;
            }
            TaskStatus status = (TaskStatus)value;
            if (status == TaskStatus.Completed)
            {
                return "LightGray";
            }
            if (status == TaskStatus.Commenced)
            {
                return "Orange";
            }
            if (status == TaskStatus.Deleted)
            {
                return "Tomato";
            }

            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class WorkflowStatusColorConverterDark : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TaskStatus))
            {
                return parameter;
            }
            TaskStatus status = (TaskStatus)value;
            if (status == TaskStatus.Completed)
            {
                return "DarkGray";
            }
            if (status == TaskStatus.Commenced)
            {
                return "DarkOrange";
            }
            if (status == TaskStatus.Deleted)
            {
                return "Red";
            }

            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CompletedColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "LightGray";
            }
            else
            {
                return parameter;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EditableColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "White";
            }
            else
            {
                return "WhiteSmoke";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    

    public class CompletedColorConverterDark : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "#36454F";
            }
            else
            {
                return parameter;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            return (Double)((DateTime)value).Ticks;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTime(System.Convert.ToInt64(value));
        }
    }

    public class ScheduleTimeToTop : IMultiValueConverter, IValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime schedulingTime = (DateTime)values[0];

            try
            {
                DiaryEntryHolderViewModel d = (DiaryEntryHolderViewModel)values[1];
                DateTime startTime, endTime;
                if (d != null)
                {
                    startTime = d.StartTime;
                    endTime = d.EndTime;
                    


                        double MinutePerPixel = (endTime - startTime).TotalMinutes / 800;





                        return ((schedulingTime - startTime).TotalMinutes / d.MinutesPerPixel);



                        

                        //return 50;
                    
                    
                    
                        return null;
                    
                }
            }
            catch(Exception ex)
            {
                
            }    
                
                return null;
            
            



        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            //First object to return is the new scheduling date time
            //second is a diaryentroholderviewmodel

            //this wont work because we can't get the instance of the diaryentryholderviewmodel from this vantage point
            throw new NotImplementedException();
        }



        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DurationToHeight : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //so the first will be the task item iself
            //the second will be the diaryentryholdeviewmodel
            if (values[0] != null)
            {
                
                try
                {
                    DiaryEntryHolderViewModel d = (DiaryEntryHolderViewModel)values[1];
                    DateTime startTime, endTime;
                    startTime = d.StartTime;
                    endTime = d.EndTime;

                    try
                    {
                        
                        Int32 num = (Int32)values[0];
                        return ((num / d.MinutesPerPixel));
                    }
                    catch (Exception ex)
                    {
                       
                        return 50;
                    }
                }
                catch
                {
                    //This seems to thow an error if the usercontrol is being movedd

                    return 50;
                }
                
            }
            else
            {
                
                
                return 50;
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemToViewText : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is BasicTask)
            {
                return ((BasicTask)value).TaskName + Environment.NewLine + ((BasicTask)value).Patient.Surname + ", " + ((BasicTask)value).Patient.GivenNames;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NonFiniteToVisible : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BasicFiniteTask)
            {
                return Visibility.Collapsed;
            }
            if (value is BasicTask)
            {
                return Visibility.Visible;
            }
            return Visibility.Visible;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTime(System.Convert.ToInt64(value));
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //The first will value is the datastoreitme
            //the second is the diaryentryholderviewmodel
            BasicFiniteTask bt = values[0] as BasicFiniteTask;
            DiaryEntryHolderViewModel d = values[1] as DiaryEntryHolderViewModel;
            if (bt.ValidCompletionTime < d.StartTime)
            {
                return Visibility.Collapsed;
            }
            if (bt.ValidCompletionTime > d.EndTime)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FiniteToVisible : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BasicFiniteTask)
            {
                return Visibility.Visible;
            }
            if (value is BasicTask)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTime(System.Convert.ToInt64(value));
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //The first will value is the datastoreitme
            //the second is the diaryentryholderviewmodel
            BasicFiniteTask bt = values[0] as BasicFiniteTask;
            DiaryEntryHolderViewModel d = values[1] as DiaryEntryHolderViewModel;
            if (bt.ValidCompletionTime < d.StartTime)
            {
                return Visibility.Collapsed;
            }
            if (bt.ValidCompletionTime > d.EndTime)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskOutOfRange : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //The first will value is the datastoreitme
            //the second is the diaryentryholderviewmodel
            try
            {
                DateTime? bt = values[0] as DateTime?;
                DiaryEntryHolderViewModel d = values[1] as DiaryEntryHolderViewModel;
                if (bt.Value < d.StartTime)
                {
                    return Visibility.Collapsed;
                }
                if (bt.Value > d.EndTime)
                {
                    return Visibility.Collapsed;
                }
            }
            catch
            {
                return Visibility.Collapsed;
            }
            

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AvailableToOpacityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return 1;
            }
            else
            {
                return 0.5;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            
            if ((bool)value)
                return 40;
            else
                return 160;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if(((int)value) > 100)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return ((DateTime)value);
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value.ToString();
            DateTime resultDateTime;
            return DateTime.TryParse(strValue, out resultDateTime) ? resultDateTime : value;
        }
    }

    public class TransportTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((PatientTransportType)value == PatientTransportType.Wheelchair)
            {
                return "/iRadiate.Desktop.Common;component/Images/WheelchairIcon.png";
            }
            else if ((PatientTransportType)value == PatientTransportType.Bed)
            {
                return "/iRadiate.Desktop.Common;component/Images/BedIcon.png";
            }
            else 
            {
                return "/iRadiate.Desktop.Common;component/Images/AmbulatoryIcon.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TransportTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((PatientTransportType)value == PatientTransportType.Wheelchair)
            {
                return new PackIconFontAwesome { Kind = PackIconFontAwesomeKind.WheelchairSolid, Width = 24, Height = 24 };
            }
            else if ((PatientTransportType)value == PatientTransportType.Bed)
            {
                return new PackIconFontAwesome { Kind = PackIconFontAwesomeKind.BedSolid, Width = 24, Height = 24 };
            }
            else
            {
                return new PackIconFontAwesome { Kind = PackIconFontAwesomeKind.WalkingSolid, Width = 24, Height = 24 };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PregnancyStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((PregnancyStatus)value == PregnancyStatus.DefinitelyPregnant)
            {
                return "/iRadiate.Desktop.Common;component/Images/PregnantIcon.png";
            }
            else if ((PregnancyStatus)value == PregnancyStatus.PossiblyPregnant)
            {
                return "/iRadiate.Desktop.Common;component/Images/PossiblyPregnantIcon.png";
            }
            else
            {
                return "/iRadiate.Desktop.Common;component/Images/NotPregnantIcon.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PregnancyStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((PregnancyStatus)value == PregnancyStatus.DefinitelyPregnant)
            {
                return new PackIconMaterial { Kind = PackIconMaterialKind.HumanPregnant, Width = 24, Height = 24 };
            }
            else if ((PregnancyStatus)value == PregnancyStatus.PossiblyPregnant)
            {
                return new PackIconMaterial { Kind = PackIconMaterialKind.HumanPregnant, Width = 24, Height = 24 };
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class GenderToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((Gender)value == Gender.Male)
            {
                return "/iRadiate.Desktop.Common;component/Images/MaleIcon.png";
            }
            else if ((Gender)value == Gender.Female)
            {
                return "/iRadiate.Desktop.Common;component/Images/FemaleIcon.png";
            }
            else
            {
                return "/iRadiate.Desktop.Common;component/Images/QuestionMarkIcon.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GenderToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Gender)value == Gender.Male)
            {
                return new PackIconMaterial { Kind = PackIconMaterialKind.GenderMale, Width=24, Height=24};
            }
            else if ((Gender)value == Gender.Female)
            {
                return new PackIconMaterial { Kind = PackIconMaterialKind.GenderFemale, Width = 24, Height = 24 };
            }
            else
            {
                return new PackIconModern { Kind = PackIconModernKind.Question, Width = 24, Height = 24 };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    public class AdministrationRouteToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            AdministrationRoute? ans = (AdministrationRoute?)value;

            return ans.Value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "")
                return null;
            iRadiate.DataModel.NucMed.AdministrationRoute x = iRadiate.DataModel.NucMed.AdministrationRoute.Intravenous;
            if (Enum.TryParse(value.ToString(), out x))
            {
                return x;
            }
            return null;
        }
    }
}
