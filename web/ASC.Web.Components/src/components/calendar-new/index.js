import React, { Component } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import ComboBox from "../combobox";
import moment from "moment";
import { Weekdays, Days } from "./sub-components";
import isEmpty from "lodash/isEmpty";

const HoverStyle = css`
  &:hover {
    background-color: #eceef1;
    border-radius: 16px;
    cursor: pointer;
  }
`;

const DisabledStyle = css`
  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -khtml-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
`;

const ComboBoxStyle = styled.div`
  position: relative;
  display: flex;
  padding-bottom: 24px !important;
`;

const ComboBoxMonthStyle = styled.div`
  ${props =>
    props.size === "base"
      ? `width: 172px; max-width: 172px;`
      : `width: 205px; max-width: 205px;`}
`;

const ComboBoxDateStyle = styled.div`
  min-width: 80px;
  height: 32px;
  margin-left: 8px;
`;

const CalendarContainer = styled.div`
  ${props =>
    props.size === "base" ? `max-width: 293px;` : `max-width: 325px;`}
`;

const CalendarStyle = styled.div`
  ${props => (props.size === "base" ? "width: 260px;" : "width: 294px;")}

  padding: 16px 16px 16px 17px;
  box-sizing: content-box;
  ${props =>
    props.isDisabled
      ? `pointer-events: none;
        ${DisabledStyle}
        `
      : "pointer-events: auto;"}

  .calendar-month {
    ${HoverStyle}
  }

  .calendar-month_neighboringMonth {
    color: #eceef1;

    ${HoverStyle}
    &:hover {
      color: #333;
    }
  }

  .calendar-month_disabled {
    ${DisabledStyle}
    color: #ECEEF1;
    pointer-events: none;
  }

  .calendar-month_weekend {
    color: #a3a9ae;
    ${HoverStyle}
  }

  .calendar-month_selected-day {
    background-color: ${props => props.color};
    border-radius: 16px;
    cursor: pointer;
    color: #fff;
  }
`;

const Month = styled.div`
  width: ${props => (props.size === "base" ? "267px" : "303px")};
`;

class Calendar extends Component {
  constructor(props) {
    super(props);

    moment.locale(props.locale);
    this.state = this.mapPropsToState(props);
  }

  mapPropsToState = props => {
    const { minDate, maxDate, openToDate, selectedDate } = props;
    const months = moment.months();
    const arrayWeekdays = moment.weekdaysMin();
    const optionsMonth = this.getListMonth(
      minDate,
      maxDate,
      openToDate,
      months
    );
    const optionsYear = this.getArrayYears(minDate, maxDate);
    const optionsDays = this.getDays(
      minDate,
      maxDate,
      openToDate,
      selectedDate
    );
    const optionsWeekdays = this.getWeekDays(arrayWeekdays);

    const newState = {
      months,
      minDate,
      maxDate,
      openToDate,
      selectedDate,
      optionsMonth,
      selectedOptionMonth: this.getCurrentMonth(optionsMonth, openToDate),
      optionsYear,
      selectedOptionYear: this.getCurrentYear(optionsYear, openToDate),
      optionsDays,
      optionsWeekdays
    };

    //console.log("mapPropsToState ", newState);
    return newState;
  };

  onSelectYear = value => {
    const openToDate = new Date(value.key, this.state.openToDate.getMonth());

    const optionsMonth = this.getListMonth(
      this.state.minDate,
      this.state.maxDate,
      openToDate,
      this.state.months
    );

    const openToDateMonth = openToDate.getMonth();
    const openToDateYear = openToDate.getFullYear();
    let selectedMonth = optionsMonth.find(x => x.key == openToDateMonth);
    let newOpenToDate = openToDate;

    if (selectedMonth.disabled === true) {
      selectedMonth = optionsMonth.find(x => x.disabled === false);
      newOpenToDate = new Date(openToDateYear, selectedMonth.key, 1);
    }

    const newState = this.mapPropsToState({
      ...this.state,
      openToDate: newOpenToDate,
      optionsMonth
    });

    this.setState(newState);
  };

  onSelectMonth = value => {
    const newState = this.mapPropsToState({
      ...this.state,
      openToDate: new Date(this.state.openToDate.getFullYear(), value.key)
    });
    //console.log("onSelectMonth", newState);
    this.setState(newState);
  };

  onDayClick = dayItem => {
    //console.log("onDayClick", dayItem);
    const day = dayItem.value;
    const currentMonth = this.state.openToDate.getMonth();
    const currentYear = this.state.openToDate.getFullYear();
    const dateInCurrentMonth = new Date(currentYear, currentMonth, day);
    let newState;

    if (dayItem.dayState === "prev") {
      const dateInPrevMonth = new Date(currentYear, currentMonth - 1, day);
      newState = this.mapPropsToState({
        ...this.state,
        selectedDate: dateInPrevMonth,
        openToDate: dateInPrevMonth
      });
    } else if (dayItem.dayState === "next") {
      const dateInNextMonth = new Date(currentYear, currentMonth + 1, day);
      newState = this.mapPropsToState({
        ...this.state,
        selectedDate: dateInNextMonth,
        openToDate: dateInNextMonth
      });
    } else if (
      this.formatSelectedDate(dateInCurrentMonth) !=
      this.formatSelectedDate(this.state.selectedDate)
    ) {
      newState = this.mapPropsToState({
        ...this.state,
        selectedDate: dateInCurrentMonth
      });
    }

    if (newState) {
      this.setState(newState);
      this.props.onChange && this.props.onChange(newState.selectedDate);
    }
  };

  getListMonth = (minDate, maxDate, openToDate, months) => {
    const minDateYear = minDate.getFullYear();
    const minDateMonth = minDate.getMonth();

    const maxDateYear = maxDate.getFullYear();
    const maxDateMonth = maxDate.getMonth();

    const openToDateYear = openToDate.getFullYear();

    let disabled = false;
    const listMonths = [];

    let i = 0;
    while (i <= 11) {
      listMonths.push({
        key: `${i}`,
        label: `${months[i]}`,
        disabled: disabled
      });
      i++;
    }

    if (openToDateYear === minDateYear) {
      i = 0;
      while (i != minDateMonth) {
        listMonths[i].disabled = true;
        i++;
      }
    } else if (openToDateYear === maxDateYear) {
      i = 11;
      while (i != maxDateMonth) {
        listMonths[i].disabled = true;
        i--;
      }
    }

    return listMonths;
  };

  getCurrentMonth = (months, openToDate) => {
    const openToDateMonth = openToDate.getMonth();
    let selectedMonth = months.find(x => x.key == openToDateMonth);

    if (selectedMonth.disabled === true) {
      selectedMonth = months.find(x => x.disabled === false);
    }

    return selectedMonth;
  };

  getArrayYears = (minDate, maxDate) => {
    const minYear = minDate.getFullYear();
    const maxYear = maxDate.getFullYear();
    const yearList = [];

    let i = minYear;
    while (i <= maxYear) {
      let newDate = new Date(i, 0, 1);
      const label = moment(newDate).format("YYYY");
      const key = i;
      yearList.push({ key, label: label });
      i++;
    }
    return yearList.reverse();
  };

  getCurrentYear = (arrayYears, openToDate) => {
    const openToDateYear = openToDate.getFullYear();
    return arrayYears.find(x => x.key == openToDateYear);
  };

  formatSelectedDate = date => {
    return (
      date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear()
    );
  };

  formatDate = date => {
    return date.getMonth() + 1 + "/" + 1 + "/" + date.getFullYear();
  };

  compareDays = (openToDate, selectedDate) => {
    return this.formatDate(openToDate) === this.formatDate(selectedDate);
  };

  firstDayOfMonth = openToDate => {
    const firstDay = moment(openToDate)
      .locale("en")
      .startOf("month")
      .format("d");
    let day = firstDay - 1;
    if (day < 0) {
      day = 6;
    }
    return day;
  };

  getWeekDays = weekdays => {
    let arrayWeekDays = [];
    weekdays.push(weekdays.shift());
    for (let i = 0; i < weekdays.length; i++) {
      arrayWeekDays.push({
        key: `${this.props.locale}_${i}`,
        value: weekdays[i],
        color: i >= 5 ? "#A3A9AE" : undefined
      });
    }
    return arrayWeekDays;
  };

  getDays = (minDate, maxDate, openToDate, selectedDate) => {
    const currentYear = openToDate.getFullYear();
    const currentMonth = openToDate.getMonth() + 1;
    const countDaysInMonth = new Date(currentYear, currentMonth, 0).getDate();
    let countDaysInPrevMonth = new Date(
      currentYear,
      currentMonth - 1,
      0
    ).getDate();
    const arrayDays = [];
    let className = "calendar-month_neighboringMonth";

    const openToDateMonth = openToDate.getMonth();
    const openToDateYear = openToDate.getFullYear();

    const maxDateMonth = maxDate.getMonth();
    const maxDateYear = maxDate.getFullYear();
    const maxDateDay = maxDate.getDate();

    const minDateMonth = minDate.getMonth();
    const minDateYear = minDate.getFullYear();
    const minDateDay = minDate.getDate();

    //Disable preview month
    let disableClass = null;
    if (openToDateYear === minDateYear && openToDateMonth === minDateMonth) {
      disableClass = "calendar-month_disabled";
    }

    //Prev month
    let prevMonthDay = null;
    if (
      openToDateYear === minDateYear &&
      openToDateMonth - 1 === minDateMonth
    ) {
      prevMonthDay = minDateDay;
    }

    //prev month + year
    let prevYearDay = null;
    if (
      openToDateYear === minDateYear + 1 &&
      openToDateMonth === 0 &&
      minDateMonth === 11
    ) {
      prevYearDay = minDateDay;
    }

    // Show neighboring days in prev month
    const firstDayOfMonth = this.firstDayOfMonth(openToDate);

    for (let i = firstDayOfMonth; i != 0; i--) {
      if (countDaysInPrevMonth + 1 === prevMonthDay) {
        disableClass = "calendar-month_disabled";
      }
      if (countDaysInPrevMonth + 1 === prevYearDay) {
        disableClass = "calendar-month_disabled";
      }
      arrayDays.unshift({
        value: countDaysInPrevMonth--,
        disableClass,
        className,
        dayState: "prev"
      });
    }

    //Disable max days in month
    let maxDay, minDay;
    disableClass = null;
    if (openToDateYear === maxDateYear && openToDateMonth >= maxDateMonth) {
      if (openToDateMonth === maxDateMonth) {
        maxDay = maxDateDay;
      } else {
        maxDay = null;
      }
    }

    //Disable min days in month
    if (openToDateYear === minDateYear && openToDateMonth >= minDateMonth) {
      if (openToDateMonth === minDateMonth) {
        minDay = minDateDay;
      } else {
        minDay = null;
      }
    }

    // Show days in month and weekend days
    let seven = 7;
    const dateNow = selectedDate.getDate();

    for (let i = 1; i <= countDaysInMonth; i++) {
      if (i === seven - firstDayOfMonth - 1) {
        className = "calendar-month_weekend";
      } else if (i === seven - firstDayOfMonth) {
        seven += 7;
        className = "calendar-month_weekend";
      } else {
        className = "calendar-month";
      }
      if (i === dateNow && this.compareDays(openToDate, selectedDate)) {
        className = "calendar-month_selected-day";
      }
      if (i > maxDay || i < minDay) {
        disableClass = "calendar-month_disabled";
        className = "calendar-month_disabled";
      } else {
        disableClass = null;
      }

      arrayDays.push({
        value: i,
        disableClass,
        className,
        dayState: "now"
      });
    }

    //Calculating neighboring days in next month
    let maxDaysInMonthTable = 42;
    const maxDaysInMonth = 42;
    if (firstDayOfMonth > 5 && countDaysInMonth >= 30) {
      maxDaysInMonthTable += 7;
    } else if (firstDayOfMonth >= 5 && countDaysInMonth > 30) {
      maxDaysInMonthTable += 7;
    }
    if (maxDaysInMonthTable > maxDaysInMonth) {
      maxDaysInMonthTable -= 7;
    }

    //Disable next month days
    disableClass = null;
    if (openToDateYear === maxDateYear && openToDateMonth >= maxDateMonth) {
      disableClass = "calendar-month_disabled";
    }

    //next month + year
    let nextYearDay = null;
    if (
      openToDateYear === maxDateYear - 1 &&
      openToDateMonth === 11 &&
      maxDateMonth === 0
    ) {
      nextYearDay = maxDateDay;
    }

    //next month
    let nextMonthDay = null;
    if (
      openToDateYear === maxDateYear &&
      openToDateMonth === maxDateMonth - 1
    ) {
      nextMonthDay = maxDateDay;
    }

    //Show neighboring days in next month
    let dayInNextMonth = 1;
    className = "calendar-month_neighboringMonth";
    for (
      let i = countDaysInMonth;
      i < maxDaysInMonthTable - firstDayOfMonth;
      i++
    ) {
      if (i - countDaysInMonth === nextYearDay) {
        disableClass = "calendar-month_disabled";
      }
      if (i - countDaysInMonth === nextMonthDay) {
        disableClass = "calendar-month_disabled";
      }
      arrayDays.push({
        value: dayInNextMonth++,
        disableClass,
        className,
        dayState: "next"
      });
    }
    return arrayDays;
  };

  componentDidUpdate(prevProps) {
    const {
      selectedDate,
      openToDate,
      minDate,
      maxDate,
      isDisabled,
      locale
    } = this.props;

    let newState = {};

    if (this.compareDates(selectedDate, prevProps.selectedDate) !== 0) {
      newState = { selectedDate };
    }

    if (this.compareDates(openToDate, prevProps.openToDate) !== 0) {
      newState = Object.assign({}, newState, {
        openToDate
      });
    }

    if (this.compareDates(minDate, prevProps.minDate) !== 0) {
      newState = Object.assign({}, newState, {
        minDate
      });
    }

    if (this.compareDates(maxDate, prevProps.maxDate) !== 0) {
      newState = Object.assign({}, newState, {
        maxDate
      });
    }

    if (isDisabled !== prevProps.isDisabled) {
      newState = Object.assign({}, newState, {
        isDisabled
      });
    }

    if (!isEmpty(newState) || locale !== prevProps.locale) {
      moment.locale(locale);
      newState = this.mapPropsToState({
        ...this.state,
        ...newState
      });
      this.setState(newState);
    }
  }

  compareDates = (date1, date2) => {
    return moment(date1)
      .startOf("day")
      .diff(moment(date2).startOf("day"), "days");
  };

  shouldComponentUpdate(nextProps) {
    const {
      selectedDate,
      openToDate,
      minDate,
      maxDate,
      isDisabled
    } = this.props;

    if (
      this.compareDates(selectedDate, nextProps.selectedDate) === 0 &&
      this.compareDates(openToDate, nextProps.openToDate) === 0 &&
      this.compareDates(minDate, nextProps.minDate) === 0 &&
      this.compareDates(maxDate, nextProps.maxDate) === 0 &&
      isDisabled !== nextProps.isDisabled
    ) {
      return false;
    }
    return true;
  }

  render() {
    //console.log("Calendar render");

    const { isDisabled, size, themeColor } = this.props;
    const {
      optionsMonth,
      selectedOptionMonth,
      selectedOptionYear,
      optionsYear,
      optionsDays,
      optionsWeekdays
    } = this.state;

    const dropDownSizeMonth = optionsMonth.length > 4 ? 184 : undefined;
    const dropDownSizeYear = optionsYear.length > 4 ? 184 : undefined;

    return (
      <CalendarContainer size={size}>
        <CalendarStyle size={size} color={themeColor} isDisabled={isDisabled}>
          <ComboBoxStyle>
            <ComboBoxMonthStyle size={size}>
              <ComboBox
                scaled={true}
                dropDownMaxHeight={dropDownSizeMonth}
                onSelect={this.onSelectMonth}
                selectedOption={selectedOptionMonth}
                options={optionsMonth}
                isDisabled={isDisabled}
              />
            </ComboBoxMonthStyle>
            <ComboBoxDateStyle>
              <ComboBox
                scaled={true}
                dropDownMaxHeight={dropDownSizeYear}
                onSelect={this.onSelectYear}
                selectedOption={selectedOptionYear}
                options={optionsYear}
                isDisabled={isDisabled}
              />
            </ComboBoxDateStyle>
          </ComboBoxStyle>

          <Month size={size}>
            <Weekdays optionsWeekdays={optionsWeekdays} size={size} />
            <Days
              optionsDays={optionsDays}
              size={size}
              onDayClick={this.onDayClick}
            />
          </Month>
        </CalendarStyle>
      </CalendarContainer>
    );
  }
}

Calendar.propTypes = {
  onChange: PropTypes.func,
  themeColor: PropTypes.string,
  selectedDate: PropTypes.instanceOf(Date),
  openToDate: PropTypes.instanceOf(Date),
  minDate: PropTypes.instanceOf(Date),
  maxDate: PropTypes.instanceOf(Date),
  locale: PropTypes.string,
  isDisabled: PropTypes.bool,
  size: PropTypes.oneOf(["base", "big"])
};

Calendar.defaultProps = {
  selectedDate: new Date(),
  openToDate: new Date(),
  minDate: new Date("1970/01/01"),
  maxDate: new Date(new Date().getFullYear() + 1 + "/01/01"),
  themeColor: "#ED7309",
  locale: moment.locale(),
  size: "base"
};

export default Calendar;
