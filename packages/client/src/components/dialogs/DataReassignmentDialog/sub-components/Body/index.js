import Progress from "./Progress";
import DataReassignmentLoader from "@docspace/common/components/Loaders/DataReassignmentLoader";
import AccountInfo from "./AccountInfo";
import Description from "./Description";
import NewOwner from "./NewOwner";

const Body = ({
  t,
  tReady,
  showProgress,
  isReassignCurrentUser,
  user,
  selectedUser,
  percent,
  currentColorScheme,
  onTogglePeopleSelector,
  onTerminate,
}) => {
  if (!tReady) return <DataReassignmentLoader />;

  if (showProgress)
    return (
      <Progress
        isReassignCurrentUser={isReassignCurrentUser}
        fromUser={user.displayName}
        toUser={
          selectedUser.displayName
            ? selectedUser.displayName
            : selectedUser.label
        }
        percent={percent}
        onTerminate={onTerminate}
      />
    );

  return (
    <>
      <AccountInfo user={user} />
      <NewOwner
        t={t}
        selectedUser={selectedUser}
        currentColorScheme={currentColorScheme}
        onTogglePeopleSelector={onTogglePeopleSelector}
      />
      <Description t={t} />
    </>
  );
};

export default Body;
