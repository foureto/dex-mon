import React from "react";
import Page from "@components/Page";

const MainPage: React.FC = () => {
  return (
    <Page title="Main" subTitle="dashboard" loading={false}>
      <>Hello</>
    </Page>
  );
};

export default MainPage;
